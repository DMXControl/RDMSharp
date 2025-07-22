using RDMSharp.Metadata;
using System.Collections.Generic;

namespace RDMSharp.RDM.Device.Module
{
    public sealed class TagsModule : AbstractModule
    {
        private List<string> tags = new List<string>();
        public IReadOnlyCollection<string> Tags
        {
            get
            {
                IReadOnlyCollection<string> tagList = null;
                if (this.ParentDevice.GetAllParameterValues().TryGetValue(ERDM_Parameter.LIST_TAGS, out object tags))
                {
                    if (tags is IReadOnlyCollection<string> _tagList)
                        tagList = _tagList;
                }
                return tagList;
            }
        }
        public TagsModule() : base(
            "Tags",
            ERDM_Parameter.LIST_TAGS,
            ERDM_Parameter.ADD_TAG,
            ERDM_Parameter.REMOVE_TAG,
            ERDM_Parameter.CHECK_TAG,
            ERDM_Parameter.CLEAR_TAGS)
        {
        }

        protected override void OnParentDeviceChanged(AbstractGeneratedRDMDevice device)
        {
            this.ParentDevice.setParameterValue(ERDM_Parameter.LIST_TAGS, tags.ToArray());
        }

        protected override void ParameterChanged(ERDM_Parameter parameter, object newValue, object index)
        {
            switch (parameter)
            {
                case ERDM_Parameter.LIST_TAGS:
                    OnPropertyChanged(nameof(Tags));
                    break;
            }
        }
        public override bool IsHandlingParameter(ERDM_Parameter parameter, ERDM_Command command)
        {
            switch (parameter)
            {
                case ERDM_Parameter.LIST_TAGS:
                    break;
                case ERDM_Parameter.ADD_TAG:
                case ERDM_Parameter.REMOVE_TAG:
                case ERDM_Parameter.CLEAR_TAGS:
                case ERDM_Parameter.CHECK_TAG:
                    return true; // These parameters are handled by this module.
            }
            return false; // Default case, not handled by this module.
        }

        protected override RDMMessage handleRequest(RDMMessage message)
        {
            switch (message.Parameter)
            {
                case ERDM_Parameter.ADD_TAG when message.Command is ERDM_Command.SET_COMMAND:
                    if (message.Value is string tag)
                    {
                        if (string.IsNullOrWhiteSpace(tag))
                        {
                            return new RDMMessage(ERDM_NackReason.FORMAT_ERROR)
                            {
                                SourceUID = message.DestUID,
                                DestUID = message.SourceUID,
                                Command = ERDM_Command.SET_COMMAND_RESPONSE,
                                Parameter = message.Parameter
                            };
                        }
                        try
                        {
#if DEBUG
                            if (tag.Equals("GeNeRaTeHaRdWaReFaUlT"))
                                throw new System.Exception("Simulated hardware fault for testing purposes.");
#endif
                            if (!validateTag(tag))
                            {
                                return new RDMMessage(ERDM_NackReason.DATA_OUT_OF_RANGE)
                                {
                                    SourceUID = message.DestUID,
                                    DestUID = message.SourceUID,
                                    Command = ERDM_Command.SET_COMMAND_RESPONSE,
                                    Parameter = message.Parameter
                                };
                            }
                            if (!tags.Contains(tag))
                                tags.Add(tag);
                            this.ParentDevice.setParameterValue(ERDM_Parameter.LIST_TAGS, tags.ToArray());
                            return new RDMMessage()
                            {
                                SourceUID = message.DestUID,
                                DestUID = message.SourceUID,
                                Command = ERDM_Command.SET_COMMAND_RESPONSE,
                                Parameter = message.Parameter
                            };
                        }
                        catch (System.Exception ex)
                        {
                            Logger?.LogError(ex);
                        }
                        break;
                    }
                    return new RDMMessage(ERDM_NackReason.FORMAT_ERROR)
                    {
                        SourceUID = message.DestUID,
                        DestUID = message.SourceUID,
                        Command = ERDM_Command.SET_COMMAND_RESPONSE,
                        Parameter = message.Parameter
                    };
                case ERDM_Parameter.REMOVE_TAG when message.Command is ERDM_Command.SET_COMMAND:
                    if (message.Value is string tag2)
                    {
                        if (string.IsNullOrWhiteSpace(tag2))
                        {
                            return new RDMMessage(ERDM_NackReason.FORMAT_ERROR)
                            {
                                SourceUID = message.DestUID,
                                DestUID = message.SourceUID,
                                Command = ERDM_Command.SET_COMMAND_RESPONSE,
                                Parameter = message.Parameter
                            };
                        }
                        try
                        {
#if DEBUG
                            if (tag2.Equals("GeNeRaTeHaRdWaReFaUlT"))
                                throw new System.Exception("Simulated hardware fault for testing purposes.");
#endif
                            if (!validateTag(tag2) || !tags.Contains(tag2))
                            {
                                return new RDMMessage(ERDM_NackReason.DATA_OUT_OF_RANGE)
                                {
                                    SourceUID = message.DestUID,
                                    DestUID = message.SourceUID,
                                    Command = ERDM_Command.SET_COMMAND_RESPONSE,
                                    Parameter = message.Parameter
                                };
                            }
                            tags.Remove(tag2);
                            this.ParentDevice.setParameterValue(ERDM_Parameter.LIST_TAGS, tags.ToArray());
                            return new RDMMessage()
                            {
                                SourceUID = message.DestUID,
                                DestUID = message.SourceUID,
                                Command = ERDM_Command.SET_COMMAND_RESPONSE,
                                Parameter = message.Parameter
                            };
                        }
                        catch (System.Exception ex)
                        {
                            Logger?.LogError(ex);
                        }
                        break;
                    }
                    return new RDMMessage(ERDM_NackReason.FORMAT_ERROR)
                    {
                        SourceUID = message.DestUID,
                        DestUID = message.SourceUID,
                        Command = ERDM_Command.SET_COMMAND_RESPONSE,
                        Parameter = message.Parameter
                    };

                case ERDM_Parameter.CHECK_TAG when message.Command is ERDM_Command.GET_COMMAND:
                    
                    if (message.Value is string tag3)
                    {
                        if (string.IsNullOrWhiteSpace(tag3))
                        {
                            return new RDMMessage(ERDM_NackReason.FORMAT_ERROR)
                            {
                                SourceUID = message.DestUID,
                                DestUID = message.SourceUID,
                                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                                Parameter = message.Parameter
                            };
                        }
                        try
                        {
#if DEBUG
                            if (tag3.Equals("GeNeRaTeHaRdWaReFaUlT"))
                                throw new System.Exception("Simulated hardware fault for testing purposes.");
#endif
                            if (!validateTag(tag3))
                            {
                                return new RDMMessage(ERDM_NackReason.DATA_OUT_OF_RANGE)
                                {
                                    SourceUID = message.DestUID,
                                    DestUID = message.SourceUID,
                                    Command = ERDM_Command.GET_COMMAND_RESPONSE,
                                    Parameter = message.Parameter
                                };
                            }
                            bool tagExists = tags.Contains(tag3);
                            return new RDMMessage()
                            {
                                SourceUID = message.DestUID,
                                DestUID = message.SourceUID,
                                Command = ERDM_Command.GET_COMMAND_RESPONSE,
                                Parameter = message.Parameter,
                                ParameterData = tagExists ? new byte[] { 0x01 } : new byte[] { 0x00 }
                            };
                        }
                        catch (System.Exception ex)
                        {
                            Logger?.LogError(ex);
                        }
                        break;
                    }
                    return new RDMMessage(ERDM_NackReason.FORMAT_ERROR)
                    {
                        SourceUID = message.DestUID,
                        DestUID = message.SourceUID,
                        Command = ERDM_Command.GET_COMMAND_RESPONSE,
                        Parameter = message.Parameter
                    };
                case ERDM_Parameter.CLEAR_TAGS when message.Command is ERDM_Command.SET_COMMAND:
                    try
                    {
#if DEBUG
                        if (message.SourceUID.Equals(new UID(0xeeee, 0xf0f0f0f0)))
                            throw new System.Exception("Simulated hardware fault for testing purposes.");
#endif
                        tags.Clear();
                        this.ParentDevice.setParameterValue(ERDM_Parameter.LIST_TAGS, tags.ToArray());
                    }
                    catch (System.Exception ex)
                    {
                        Logger?.LogError(ex);
                        break;
                    }
                    return new RDMMessage()
                    {
                        SourceUID = message.DestUID,
                        DestUID = message.SourceUID,
                        Command = ERDM_Command.SET_COMMAND_RESPONSE,
                        Parameter = message.Parameter
                    };
                case ERDM_Parameter.ADD_TAG:
                case ERDM_Parameter.REMOVE_TAG:
                case ERDM_Parameter.CHECK_TAG:
                case ERDM_Parameter.CLEAR_TAGS:
                    return new RDMMessage(ERDM_NackReason.UNSUPPORTED_COMMAND_CLASS)
                    {
                        SourceUID = message.DestUID,
                        DestUID = message.SourceUID,
                        Command = message.Command | ERDM_Command.RESPONSE,
                        Parameter = message.Parameter
                    };
            }
            return new RDMMessage(ERDM_NackReason.HARDWARE_FAULT)
            {
                SourceUID = message.DestUID,
                DestUID = message.SourceUID,
                Command = message.Command | ERDM_Command.RESPONSE,
                Parameter = message.Parameter
            };
        }

        public void AddTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new System.ArgumentException("Tag can't be null or whitespace.", nameof(tag));
            if (!validateTag(tag))
                throw new System.ArgumentOutOfRangeException(nameof(tag), "Tag must not exceed 32 characters.");
            if (!tags.Contains(tag))
            {
                tags.Add(tag);
                this.ParentDevice.setParameterValue(ERDM_Parameter.LIST_TAGS, tags.ToArray());
            }
        }
        public void RemoveTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new System.ArgumentException("Tag can't be null or whitespace.", nameof(tag));
            if (!validateTag(tag))
                throw new System.ArgumentOutOfRangeException(nameof(tag), "Tag must not exceed 32 characters.");
            if (tags.Contains(tag))
            {
                tags.Remove(tag);
                this.ParentDevice.setParameterValue(ERDM_Parameter.LIST_TAGS, tags.ToArray());
            }
        }
        public bool CheckTag(string tag)
        {
            if (string.IsNullOrWhiteSpace(tag))
                throw new System.ArgumentException("Tag can't be null or whitespace.", nameof(tag));
            if (!validateTag(tag))
                throw new System.ArgumentOutOfRangeException(nameof(tag), "Tag must not exceed 32 characters.");
            return tags.Contains(tag);
        }
        public void ClearTags()
        {
            tags.Clear();
            this.ParentDevice.setParameterValue(ERDM_Parameter.LIST_TAGS, tags.ToArray());
        }
        private bool validateTag(string tag)
        {
            return tag.Length <= 32;
        }
    }
}