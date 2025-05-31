namespace RDMSharp
{
    public readonly struct ParameterDataCacheBag
    {
        public readonly ERDM_Parameter Parameter;
        public readonly object Index;

        public ParameterDataCacheBag(ERDM_Parameter parameter, object index)
        {
            Parameter = parameter;
            Index = index;
        }
        public ParameterDataCacheBag(ERDM_Parameter parameter)
        {
            Parameter = parameter;
        }

        public override string ToString()
        {
            if (Index == null)
                return $"{Parameter}";
            else
                return $"{Parameter} ({Index})";
        }
    }
}