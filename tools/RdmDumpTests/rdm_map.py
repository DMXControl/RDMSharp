import json

def num(v):
    return int(v, 0) if isinstance(v, str) else int(v)

def cs_str(v):
    out = []
    for ch in v:
        if ch == '\\':
            out.append('\\\\')
        elif ch == '"':
            out.append('\\"')
        elif 32 <= ord(ch) < 127:
            out.append(ch)
        else:
            out.append('\\u%04X' % ord(ch))
    return '"' + ''.join(out) + '"'

def uid(v):
    b = bytes.fromhex(v.replace(':', ''))
    return f'new UID(0x{int.from_bytes(b[:2], "big"):04X}, 0x{int.from_bytes(b[2:], "big"):08X})'

def tenths(v):
    return f'{num(v) / 10:.1f}'

# value converters: dissector string -> C# literal
I = lambda t: (lambda v: f'({t}){num(v)}')
E = lambda t: (lambda v: f'({t})0x{num(v):02X}')
B = lambda v: 'true' if num(v) else 'false'
S = cs_str
T = tenths

PD = 'rdm.pd.'
# (parameter, command) -> (C# type, [(dissector field, property or None for the parsed object itself, converter, comparison)])
MAP = {
 ('DEVICE_INFO', 'GET_COMMAND_RESPONSE'): ('RDMDeviceInfo', [
    ('proto_vers', 'RdmProtocolVersionMajor', lambda v: f'(byte){num(v) >> 8}', None),
    ('proto_vers', 'RdmProtocolVersionMinor', lambda v: f'(byte){num(v) & 0xFF}', None),
    ('device_model_id', 'DeviceModelId', I('ushort'), None),
    ('product_cat', 'ProductCategoryCoarse', lambda v: f'(ERDM_ProductCategoryCoarse)0x{num(v) >> 8:02X}', None),
    ('product_cat', 'ProductCategoryFine', lambda v: f'(ERDM_ProductCategoryFine)0x{num(v):04X}', None),
    ('software_version_id', 'SoftwareVersionId', lambda v: f'0x{num(v):08X}u', None),
    ('dmx_footprint', 'Dmx512Footprint', I('ushort'), None),
    ('dmx_pers_current', 'Dmx512CurrentPersonality', I('byte'), None),
    ('dmx_pers_total', 'Dmx512NumberOfPersonalities', I('byte'), None),
    ('dmx_start_address', 'Dmx512StartAddress', I('ushort'), None),
    ('sub_device_count', 'SubDeviceCount', I('ushort'), None),
    ('sensor_count', 'SensorCount', I('byte'), None)]),
 ('SUPPORTED_PARAMETERS', 'GET_COMMAND_RESPONSE'): ('ERDM_Parameter[]', 'LIST:parameter.id:ERDM_Parameter'),
 ('PRODUCT_DETAIL_ID_LIST', 'GET_COMMAND_RESPONSE'): ('ERDM_ProductDetail[]', 'LIST:product_detail_id:ERDM_ProductDetail'),
 ('DEVICE_MODEL_DESCRIPTION', 'GET_COMMAND_RESPONSE'): ('string', [('device_model_description', None, S, None)]),
 ('MANUFACTURER_LABEL', 'GET_COMMAND_RESPONSE'): ('string', [('manu_label', None, S, None)]),
 ('DEVICE_LABEL', 'GET_COMMAND_RESPONSE'): ('string', [('device_label', None, S, None)]),
 ('SOFTWARE_VERSION_LABEL', 'GET_COMMAND_RESPONSE'): ('string', [('software_version.label', None, S, None)]),
 ('BOOT_SOFTWARE_VERSION_LABEL', 'GET_COMMAND_RESPONSE'): ('string', [('boot_software_version.label', None, S, None)]),
 ('BOOT_SOFTWARE_VERSION_ID', 'GET_COMMAND_RESPONSE'): ('uint', [('boot_software_version.id', None, lambda v: f'0x{num(v):08X}u', None)]),
 ('DMX_PERSONALITY_DESCRIPTION', 'GET_COMMAND'): ('byte', [('dmx_pers.requested', None, I('byte'), None)]),
 ('DMX_PERSONALITY_DESCRIPTION', 'GET_COMMAND_RESPONSE'): ('RDMDMXPersonalityDescription', [
    ('dmx_pers.requested', 'PersonalityId', I('byte'), None),
    ('dmx_pers.slots', 'Slots', I('ushort'), None),
    ('dmx_pers.text', 'Description', S, None)]),
 ('PROXIED_DEVICES', 'GET_COMMAND_RESPONSE'): ('RDMProxiedDevices', 'UIDS:proxied_devices.uid:Devices'),
 ('PROXIED_DEVICES_COUNT', 'GET_COMMAND_RESPONSE'): ('RDMProxiedDeviceCount', [
    ('device_count', 'DeviceCount', I('ushort'), None),
    ('list_change', 'ListChange', B, None)]),
 ('PARAMETER_DESCRIPTION', 'GET_COMMAND'): ('ERDM_Parameter', [('parameter.id', None, lambda v: f'(ERDM_Parameter)0x{num(v):04X}', None)]),
 ('PARAMETER_DESCRIPTION', 'GET_COMMAND_RESPONSE'): ('RDMParameterDescription', [
    ('parameter.id', 'ParameterId', lambda v: f'(ushort)0x{num(v):04X}', None),
    ('parameter.pdl_size', 'PDLSize', I('byte'), None),
    ('parameter.data_type', 'DataType', E('ERDM_DataType'), None),
    ('parameter.cmd_class', 'CommandClass', E('ERDM_CommandClass'), None),
    ('parameter.type', 'Type', I('byte'), None),
    ('parameter.unit', 'Unit', E('ERDM_SensorUnit'), None),
    ('parameter.prefix', 'Prefix', E('ERDM_UnitPrefix'), None),
    ('parameter.min_value', 'MinValidValue', lambda v: f'{num(v)}u', None),
    ('parameter.max_value', 'MaxValidValue', lambda v: f'{num(v)}u', None),
    ('parameter.default_value', 'DefaultValue', lambda v: f'{num(v)}u', None),
    ('parameter.description', 'Description', S, None)]),
 ('DMX_PERSONALITY', 'GET_COMMAND_RESPONSE'): ('RDMDMXPersonality', [
    ('dmx_pers_current', 'CurrentPersonality', I('byte'), None),
    ('dmx_pers.count', 'OfPersonalities', I('byte'), None)]),
 ('IDENTIFY_DEVICE', 'GET_COMMAND_RESPONSE'): ('bool', [('identify_device.state', None, B, None)]),
 ('SENSOR_DEFINITION', 'GET_COMMAND'): ('byte', [('sensor.nr', None, I('byte'), None)]),
 ('SENSOR_DEFINITION', 'GET_COMMAND_RESPONSE'): ('RDMSensorDefinition', [
    ('sensor.nr', 'SensorId', I('byte'), None),
    ('sensor_type', 'Type', E('ERDM_SensorType'), None),
    ('sensor_unit', 'Unit', E('ERDM_SensorUnit'), None),
    ('sensor_prefix', 'Prefix', E('ERDM_UnitPrefix'), None),
    ('sensor.range.min_value', 'RangeMinimum', I('short'), None),
    ('sensor.range.max_value', 'RangeMaximum', I('short'), None),
    ('sensor.normal.min_value', 'NormalMinimum', I('short'), None),
    ('sensor.normal.max_value', 'NormalMaximum', I('short'), None),
    ('sensor.recorded_value_support.recorded', 'RecordedValueSupported', B, None),
    ('sensor.recorded_value_support.low_high', 'LowestHighestValueSupported', B, None),
    ('sensor.description', 'Description', S, None)]),
 ('SENSOR_VALUE', 'GET_COMMAND'): ('byte', [('sensor.nr', None, I('byte'), None)]),
 ('SENSOR_VALUE', 'GET_COMMAND_RESPONSE'): ('RDMSensorValue', [
    ('sensor.nr', 'SensorId', I('byte'), None),
    ('sensor.value.present', 'PresentValue', I('short'), None),
    ('sensor.value.lowest', 'LowestValue', I('short'), None),
    ('sensor.value.highest', 'HighestValue', I('short'), None),
    ('sensor.value.recorded', 'RecordedValue', I('short'), None)]),
 ('LOCK_STATE_DESCRIPTION', 'GET_COMMAND'): ('byte', [('lock_state_description.lock_state', None, I('byte'), None)]),
 ('LOCK_STATE_DESCRIPTION', 'GET_COMMAND_RESPONSE'): ('RDMLockStateDescription', [
    ('lock_state_description.lock_state', 'LockStateId', I('byte'), None),
    ('lock_state_description.description', 'Description', S, None)]),
 ('ENDPOINT_TIMING_DESCRIPTION', 'GET_COMMAND'): ('byte', [('endpoint_timing_description.setting', None, I('byte'), None)]),
 ('ENDPOINT_TIMING_DESCRIPTION', 'GET_COMMAND_RESPONSE'): ('GetEndpointTimingDescriptionResponse', [
    ('endpoint_timing_description.setting', 'TimingId', I('byte'), None),
    ('endpoint_timing_description.description', 'Description', S, None)]),
 ('PRESET_INFO', 'GET_COMMAND_RESPONSE'): ('RDMPresetInfo', [
    ('preset_info.level_field_supported', 'LevelFieldSupported', B, None),
    ('preset_info.preset_sequence_supported', 'PresetSequenceSupported', B, None),
    ('preset_info.split_times_supported', 'SplitTimesSupported', B, None),
    ('preset_info.dmx_fail_infinite_delay_time_supported', 'DMX512FailInfiniteDelayTimeSupported', B, None),
    ('preset_info.dmx_fail_infinite_hold_time_supported', 'DMX512FailInfiniteHoldTimeSupported', B, None),
    ('preset_info.start_up_infinite_hold_time_supported', 'StartupInfiniteHoldTimeSupported', B, None),
    ('preset_info.maximum_scene_number', 'MaximumSceneNumber', I('ushort'), None),
    ('preset_info.minimum_preset_fade_time_supported', 'MinimumPresetFadeTimeSupported', T, 'D'),
    ('preset_info.maximum_preset_fade_time_supported', 'MaximumPresetFadeTimeSupported', T, 'D'),
    ('preset_info.minimum_preset_wait_time_supported', 'MinimumPresetWaitTimeSupported', T, 'D'),
    ('preset_info.maximum_preset_wait_time_supported', 'MaximumPresetWaitTimeSupported', T, 'D'),
    ('preset_info.minimum_dmx_fail_delay_time_supported', 'MinimumDMX512FailDelayTimeSupported', T, 'D'),
    ('preset_info.maximum_dmx_fail_delay_time_supported', 'MaximumDMX512FailDelayTimeSupported', T, 'D'),
    ('preset_info.minimum_dmx_fail_hold_time_supported', 'MinimumDMX512FailDelayHoldSupported', T, 'D'),
    ('preset_info.maximum_dmx_fail_hold_time_supported', 'MaximumDMX512FailDelayHoldSupported', T, 'D'),
    ('preset_info.minimum_start_up_delay_time_supported', 'MinimumStartupDelayTimeSupported', T, 'D'),
    ('preset_info.maximum_start_up_delay_time_supported', 'MaximumStartupDelayTimeSupported', T, 'D'),
    ('preset_info.minimum_start_up_hold_time_supported', 'MinimumStartupDelayHoldSupported', T, 'D'),
    ('preset_info.maximum_start_up_hold_time_supported', 'MaximumStartupDelayHoldSupported', T, 'D')]),
 ('STATUS_MESSAGES', 'GET_COMMAND'): ('ERDM_Status', [('status_messages.type', None, E('ERDM_Status'), None)]),
 ('STATUS_MESSAGES', 'GET_COMMAND_RESPONSE'): ('RDMStatusMessage[]', 'EMPTY'),
 ('QUEUED_MESSAGE', 'GET_COMMAND'): ('ERDM_Status', [('queued_message.status', None, E('ERDM_Status'), None)]),
 ('FACTORY_DEFAULTS', 'GET_COMMAND_RESPONSE'): ('bool', [('factory_defaults', None, B, None)]),
 ('DMX_FAIL_MODE', 'GET_COMMAND_RESPONSE'): ('RDMDMX_xxxx_Mode', [
    ('dmx_fail_mode.scene_number', 'Scene', I('ushort'), None),
    ('dmx_fail_mode.loss_of_signal_delay', 'Delay', T, 'D'),
    ('dmx_fail_mode.hold_time', 'HoldTime', T, 'D'),
    ('dmx_fail_mode.level', 'Level', I('byte'), None)]),
 ('DMX_STARTUP_MODE', 'GET_COMMAND_RESPONSE'): ('RDMDMX_xxxx_Mode', [
    ('dmx_startup_mode.scene_number', 'Scene', I('ushort'), None),
    ('dmx_startup_mode.startup_delay_time', 'Delay', T, 'D'),
    ('dmx_startup_mode.hold_time', 'HoldTime', T, 'D'),
    ('dmx_startup_mode.level', 'Level', I('byte'), None)]),
 ('DISPLAY_LEVEL', 'GET_COMMAND_RESPONSE'): ('byte', [('display_level', None, I('byte'), None)]),
 ('LOCK_STATE', 'GET_COMMAND_RESPONSE'): ('GetLockStateResponse', [
    ('lock_state.lock_state', 'CurrentLockStateId', I('byte'), None),
    ('lock_state.number_of_lock_states', 'LockStates', I('byte'), None)]),
 ('ENDPOINT_LIST', 'GET_COMMAND_RESPONSE'): ('GetEndpointListResponse', 'ENDPOINTS'),
 ('ENDPOINT_LIST_CHANGE', 'GET_COMMAND_RESPONSE'): ('uint', [('endpoint_list_change.change_number', None, lambda v: f'{num(v)}u', None)]),
 ('IDENTIFY_ENDPOINT', 'GET_COMMAND_RESPONSE'): ('GetSetIdentifyEndpoint', [
    ('identify_endpoint.endpoint_id', 'EndpointId', I('ushort'), None),
    ('identify_endpoint.identify_state', 'IdentifyState', B, None)]),
 ('ENDPOINT_TO_UNIVERSE', 'GET_COMMAND_RESPONSE'): ('GetSetEndpointToUniverse', [
    ('endpoint_to_universe.endpoint_id', 'EndpointId', I('ushort'), None),
    ('endpoint_to_universe.universe_number', 'Universe', I('ushort'), None)]),
 ('ENDPOINT_MODE', 'GET_COMMAND_RESPONSE'): ('GetSetEndpointMode', [
    ('endpoint_mode.endpoint_id', 'EndpointId', I('ushort'), None),
    ('endpoint_mode.endpoint_mode', 'EndpointMode', E('ERDM_EndpointMode'), None)]),
 ('ENDPOINT_LABEL', 'GET_COMMAND_RESPONSE'): ('GetSetEndpointLabel', [
    ('endpoint_label.endpoint_id', 'EndpointId', I('ushort'), None),
    ('endpoint_label.label', 'EndpointLabel', S, None)]),
 ('RDM_TRAFFIC_ENABLE', 'GET_COMMAND_RESPONSE'): ('GetSetEndpointRDMTrafficEnable', [
    ('rdm_traffic_enable.endpoint_id', 'EndpointId', I('ushort'), None),
    ('rdm_traffic_enable.enabled', 'RDMTrafficEnabled', B, None)]),
 ('DISCOVERY_STATE', 'GET_COMMAND_RESPONSE'): ('GetDiscoveryStateResponse', [
    ('discovery_state.endpoint_id', 'EndpointId', I('ushort'), None),
    ('discovery_state.device_count', 'DeviceCount', I('ushort'), None),
    ('discovery_state.state', 'DiscoveryState', E('ERDM_DiscoveryState'), None)]),
 ('ENDPOINT_TIMING', 'GET_COMMAND_RESPONSE'): ('GetEndpointTimingResponse', [
    ('endpoint_timing.endpoint_id', 'EndpointId', I('ushort'), None),
    ('endpoint_timing.setting', 'TimingId', I('byte'), None),
    ('endpoint_timing.number_of_settings', 'Timings', I('byte'), None)]),
 ('ENDPOINT_RESPONDERS', 'GET_COMMAND_RESPONSE'): ('GetEndpointRespondersResponse', 'RESPONDERS'),
 ('ENDPOINT_RESPONDER_LIST_CHANGE', 'GET_COMMAND_RESPONSE'): ('GetEndpointResponderListChangeResponse', [
    ('endpoint_responder_list_change.endpoint_id', 'EndpointId', I('ushort'), None),
    ('endpoint_responder_list_change.change_number', 'ListChangeNumber', lambda v: f'{num(v)}u', None)]),
 ('POWER_STATE', 'GET_COMMAND_RESPONSE'): ('ERDM_PowerState', [('power_state', None, E('ERDM_PowerState'), None)]),
 ('PRESET_STATUS', 'GET_COMMAND'): ('ushort', [('preset_status.scene_number', None, I('ushort'), None)]),
 ('PRESET_STATUS', 'GET_COMMAND_RESPONSE'): ('GetPresetStatus', [
    ('preset_status.scene_number', 'SceneId', I('ushort'), None),
    ('preset_status.up_fade_time', 'UpFadeTime', T, 'D'),
    ('preset_status.down_fade_time', 'DownFadeTime', T, 'D'),
    ('preset_status.wait_time', 'WaitTime', T, 'D'),
    ('preset_status.programmed', 'Programmed', I('byte'), None)]),
}
# requests that only carry the Endpoint ID
for p, f in [('IDENTIFY_ENDPOINT', 'identify_endpoint'), ('ENDPOINT_TO_UNIVERSE', 'endpoint_to_universe'), ('ENDPOINT_MODE', 'endpoint_mode'),
             ('ENDPOINT_LABEL', 'endpoint_label'), ('RDM_TRAFFIC_ENABLE', 'rdm_traffic_enable'), ('DISCOVERY_STATE', 'discovery_state'),
             ('ENDPOINT_TIMING', 'endpoint_timing'), ('ENDPOINT_RESPONDERS', 'endpoint_responders'), ('ENDPOINT_RESPONDER_LIST_CHANGE', 'endpoint_responder_list_change')]:
    MAP[(p, 'GET_COMMAND')] = ('ushort', [(f + '.endpoint_id', None, I('ushort'), None)])

