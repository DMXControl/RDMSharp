using RDMSharp.Metadata;
using System.Collections.Concurrent;
using System.Reflection;
using System.Runtime.Loader;

namespace RDMSharpTests.Metadata;

/// <summary>
/// The metadata is loaded on the first access. Many threads accessing <see cref="MetadataFactory"/> at the same time
/// as the very first access must not see half loaded metadata (seen in ArtNetSharp: the first GET DEVICE_INFO failed with DefineNotFoundException).
/// RDMSharp is loaded into its own <see cref="AssemblyLoadContext"/> for every round, so the static state of the factory is fresh.
/// </summary>
public class TestMetadataFactoryFirstAccess
{
    private const int THREADS = 16;
    private const int ROUNDS = 3;

    private sealed class FreshFactory : IDisposable
    {
        private readonly AssemblyLoadContext context;
        private readonly MethodInfo getDefine;
        private readonly MethodInfo getMetadataDefineVersions;
        private readonly MethodInfo getDefinedDataTreeObjectType;
        private readonly Type manufacturerType;
        private readonly Type commandDublicateType;
        private readonly ConstructorInfo parameterBagConstructor;
        private readonly Type parameterType;

        public FreshFactory(int round)
        {
            context = new AssemblyLoadContext($"{nameof(TestMetadataFactoryFirstAccess)}{round}", isCollectible: true);
            // the dependencies of RDMSharp are resolved by the default context, only RDMSharp itself is loaded again
            var assembly = context.LoadFromAssemblyPath(typeof(MetadataFactory).Assembly.Location);
            var factory = assembly.GetType(typeof(MetadataFactory).FullName!)!;
            var parameterBag = assembly.GetType(typeof(ParameterBag).FullName!)!;
            parameterType = assembly.GetType(typeof(ERDM_Parameter).FullName!)!;
            getDefine = factory.GetMethod(nameof(MetadataFactory.GetDefine), new[] { parameterBag })!;
            getMetadataDefineVersions = factory.GetMethod(nameof(MetadataFactory.GetMetadataDefineVersions))!;
            manufacturerType = assembly.GetType(typeof(EManufacturer).FullName!)!;
            commandDublicateType = assembly.GetType(typeof(RDMSharp.Metadata.JSON.Command.ECommandDublicate).FullName!)!;
            getDefinedDataTreeObjectType = factory.GetMethod(nameof(MetadataFactory.GetDefinedDataTreeObjectType), new[] { manufacturerType, parameterType, commandDublicateType })!;
            parameterBagConstructor = parameterBag.GetConstructor(new[] { parameterType, typeof(ushort), typeof(ushort?), typeof(uint?) })!;
        }

        public object GetDefine(ERDM_Parameter parameter)
        {
            object bag = parameterBagConstructor.Invoke(new object?[] { Enum.ToObject(parameterType, (ushort)parameter), (ushort)0, null, null });
            return getDefine.Invoke(null, new[] { bag })!;
        }
        public Type? GetDefinedDataTreeObjectType(ERDM_Parameter parameter, RDMSharp.Metadata.JSON.Command.ECommandDublicate command)
        {
            return (Type?)getDefinedDataTreeObjectType.Invoke(null, new[] {
                Enum.ToObject(manufacturerType, (ushort)EManufacturer.ESTA),
                Enum.ToObject(parameterType, (ushort)parameter),
                Enum.ToObject(commandDublicateType, (int)command) });
        }
        public int GetMetadataDefineVersionsCount()
        {
            return ((System.Collections.ICollection)getMetadataDefineVersions.Invoke(null, null)!).Count;
        }

        public void Dispose()
        {
            context.Unload();
        }
    }

    private static List<Exception> runInParallel(Action action)
    {
        var exceptions = new ConcurrentBag<Exception>();
        using var start = new ManualResetEventSlim();
        var threads = Enumerable.Range(0, THREADS).Select(_ => new Thread(() =>
        {
            start.Wait();
            try
            {
                action();
            }
            catch (TargetInvocationException e)
            {
                exceptions.Add(e.InnerException ?? e);
            }
            catch (Exception e)
            {
                exceptions.Add(e);
            }
        })).ToList();
        threads.ForEach(t => t.Start());
        start.Set();
        foreach (var thread in threads)
            Assert.That(thread.Join(TimeSpan.FromSeconds(60)), Is.True, "the first access must not hang");
        return exceptions.ToList();
    }

    [Test]
    public void TestParallelFirstGetDefine()
    {
        for (int round = 0; round < ROUNDS; round++)
        {
            using var factory = new FreshFactory(round);
            var exceptions = runInParallel(() =>
            {
                var define = factory.GetDefine(ERDM_Parameter.DEVICE_INFO);
                if (define is null)
                    throw new Exception("DEVICE_INFO define is null");
            });
            Assert.That(exceptions.Select(e => $"{e.GetType().Name}: {e.Message}"), Is.Empty, $"round {round}");
        }
    }

    [Test]
    public void TestParallelFirstGetMetadataDefineVersions()
    {
        int expected = MetadataFactory.GetMetadataDefineVersions().Count;
        Assert.That(expected, Is.GreaterThan(0));
        for (int round = 0; round < ROUNDS; round++)
        {
            using var factory = new FreshFactory(round);
            var counts = new ConcurrentBag<int>();
            var exceptions = runInParallel(() => counts.Add(factory.GetMetadataDefineVersionsCount()));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(exceptions.Select(e => $"{e.GetType().Name}: {e.Message}"), Is.Empty, $"round {round}");
                Assert.That(counts, Is.All.EqualTo(expected), $"round {round}: every caller has to see the complete list");
            }
        }
    }

    /// <summary>
    /// The types of the parsed objects are searched on the first parse. Seen live: 3 DEVICE_INFO responses of different devices arrived
    /// at the same time and could not be parsed, because the list of the types was read while it was still filled.
    /// </summary>
    [Test]
    public void TestParallelFirstGetDefinedDataTreeObjectType()
    {
        for (int round = 0; round < ROUNDS; round++)
        {
            using var factory = new FreshFactory(round);
            var names = new ConcurrentBag<string?>();
            var exceptions = runInParallel(() => names.Add(factory.GetDefinedDataTreeObjectType(ERDM_Parameter.DEVICE_INFO, RDMSharp.Metadata.JSON.Command.ECommandDublicate.GetResponse)?.FullName));
            using (Assert.EnterMultipleScope())
            {
                Assert.That(exceptions.Select(e => $"{e.GetType().Name}: {e.Message}"), Is.Empty, $"round {round}");
                Assert.That(names, Has.Count.EqualTo(THREADS), $"round {round}");
                Assert.That(names, Is.All.EqualTo(typeof(RDMSharp.PayloadObject.RDMDeviceInfo).FullName), $"round {round}: every caller has to find the type");
            }
        }
    }
}
