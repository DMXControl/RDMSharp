"""Further parameters, found in the other dumps. values/first are injected by the generator."""
from rdm_map import MAP, num, I, E, B, S, T

values = None
first = None

def _simple(t, field, conv):
    return (t, [(field, None, conv, None)])

U = lambda v: f'{num(v)}u'

def _list(fl, key):
    return values(fl, key)

MAP.update({
 ('CAPTURE_PRESET', 'SET_COMMAND'): ('RDMCapturePreset', [
    ('capture_preset.scene_nr', 'SceneId', I('ushort'), None),
    ('capture_preset.up_fade_time', 'UpFadeTime', T, 'D'),
    ('capture_preset.down_fade_time', 'DownFadeTime', T, 'D'),
    ('capture_preset.wait_time', 'WaitTime', T, 'D')]),
 ('COMMS_STATUS', 'GET_COMMAND_RESPONSE'): ('RDMCommunicationStatus', [
    ('comms_status.short_msg', 'ShortMessage', I('ushort'), None),
    ('comms_status.len_mismatch', 'LengthMismatch', I('ushort'), None),
    ('comms_status.csum_fail', 'ChecksumFail', I('ushort'), None)]),
 ('CURVE', 'GET_COMMAND_RESPONSE'): ('RDMCurve', [
    ('curve.curve', 'CurrentCurveId', I('byte'), None),
    ('curve.number_of_curves', 'Curves', I('byte'), None)]),
 ('CURVE', 'SET_COMMAND'): _simple('byte', 'curve.curve', I('byte')),
 ('CURVE_DESCRIPTION', 'GET_COMMAND'): _simple('byte', 'curve_description.curve', I('byte')),
 ('CURVE_DESCRIPTION', 'GET_COMMAND_RESPONSE'): ('RDMCurveDescription', [
    ('curve_description.curve', 'CurveId', I('byte'), None),
    ('curve_description.description', 'Description', S, None)]),
 ('DEFAULT_SLOT_VALUE', 'GET_COMMAND_RESPONSE'): ('RDMDefaultSlotValue[]', lambda fl: [
    'Assert.That(obj.Select(s => s.SlotOffset), Is.EqualTo(new ushort[] { ' + ', '.join(str(num(v)) for v in _list(fl, 'slot_offset')) + ' }));',
    'Assert.That(obj.Select(s => s.DefaultSlotValue), Is.EqualTo(new byte[] { ' + ', '.join(str(num(v)) for v in _list(fl, 'slot_value')) + ' }));']),
 ('DEVICE_HOURS', 'GET_COMMAND_RESPONSE'): _simple('uint', 'device_hours', U),
 ('DEVICE_POWER_CYCLES', 'GET_COMMAND_RESPONSE'): _simple('uint', 'device_power_cycles', U),
 ('DEVICE_UNIT_NUMBER', 'GET_COMMAND_RESPONSE'): _simple('uint', 'device_unit_number', U),
 ('LAMP_HOURS', 'GET_COMMAND_RESPONSE'): _simple('uint', 'lamp_hours', U),
 ('LAMP_STRIKES', 'GET_COMMAND_RESPONSE'): _simple('uint', 'lamp_strikes', U),
 ('LAMP_STATE', 'GET_COMMAND_RESPONSE'): _simple('ERDM_LampState', 'lamp_state', E('ERDM_LampState')),
 ('LAMP_ON_MODE', 'GET_COMMAND_RESPONSE'): _simple('ERDM_LampMode', 'lamp_on_mode', E('ERDM_LampMode')),
 ('DEVICE_LABEL', 'SET_COMMAND'): _simple('string', 'device_label', S),
 ('DIMMER_INFO', 'GET_COMMAND_RESPONSE'): ('RDMDimmerInfo', [
    ('dimmer_info.minimum_level_lower_limit', 'MinimumLevelLowerLimit', I('ushort'), None),
    ('dimmer_info.minimum_level_upper_limit', 'MinimumLevelUpperLimit', I('ushort'), None),
    ('dimmer_info.maximum_level_lower_limit', 'MaximumLevelLowerLimit', I('ushort'), None),
    ('dimmer_info.maximum_level_upper_limit', 'MaximumLevelUpperLimit', I('ushort'), None),
    ('dimmer_info.number_of_supported_curves', 'NumberOfSupportedCurves', I('byte'), None),
    ('dimmer_info.levels_resolution', 'LevelsResolution', I('byte'), None),
    ('dimmer_info.minimum_level_split_levels_supported', 'MinimumLevelSplitLevelsSupported', B, None)]),
 ('DISPLAY_INVERT', 'GET_COMMAND_RESPONSE'): _simple('ERDM_DisplayInvert', 'display_invert', E('ERDM_DisplayInvert')),
 ('DISPLAY_INVERT', 'SET_COMMAND'): _simple('ERDM_DisplayInvert', 'display_invert', E('ERDM_DisplayInvert')),
 ('DMX_BLOCK_ADDRESS', 'GET_COMMAND_RESPONSE'): ('RDMDMXBlockAddress', [
    ('dmx_block_address.subdevice_footprint', 'TotalSubDeviceFootprint', I('ushort'), None),
    ('dmx_block_address.base_dmx_address', 'BaseDMX512Address', I('ushort'), None)]),
 ('DMX_PERSONALITY', 'SET_COMMAND'): _simple('byte', 'dmx_pers.nr', I('byte')),
 ('DMX_START_ADDRESS', 'GET_COMMAND_RESPONSE'): _simple('ushort', 'dmx_start_address', I('ushort')),
 ('DMX_START_ADDRESS', 'SET_COMMAND'): _simple('ushort', 'dmx_start_address', I('ushort')),
 ('DNS_IPV4_NAME_SERVER', 'GET_COMMAND'): _simple('byte', 'dns_ipv4_name_server.index', I('byte')),
 ('IDENTIFY_DEVICE', 'SET_COMMAND'): _simple('bool', 'identify_device', B),
 ('IDENTIFY_ENDPOINT', 'SET_COMMAND'): ('GetSetIdentifyEndpoint', [
    ('identify_endpoint.endpoint_id', 'EndpointId', I('ushort'), None),
    ('identify_endpoint.identify_state', 'IdentifyState', B, None)]),
 ('IDENTIFY_ENDPOINT', 'SET_COMMAND_RESPONSE'): _simple('ushort', 'identify_endpoint.endpoint_id', I('ushort')),
 ('IDENTIFY_MODE', 'GET_COMMAND_RESPONSE'): _simple('ERDM_IdentifyMode', 'identify_mode.identify_mode', E('ERDM_IdentifyMode')),
 ('INTERFACE_HARDWARE_ADDRESS_TYPE', 'GET_COMMAND'): _simple('uint', 'hardware_address_type1.interface_identifier', U),
 ('INTERFACE_LABEL', 'GET_COMMAND'): _simple('uint', 'interface_label.interface_identifier', U),
 ('INTERFACE_LABEL', 'GET_COMMAND_RESPONSE'): ('GetInterfaceNameResponse', [
    ('interface_label.interface_identifier', 'InterfaceId', U, None),
    ('interface_label.label', 'Label', S, None)]),
 ('IPV4_CURRENT_ADDRESS', 'GET_COMMAND'): _simple('uint', 'current_address.interface_identifier', U),
 ('IPV4_CURRENT_ADDRESS', 'GET_COMMAND_RESPONSE'): ('GetIPv4CurrentAddressResponse', [
    ('current_address.interface_identifier', 'InterfaceId', U, None),
    ('current_address.ipv4_address', 'IPAddress', lambda v: 'new IPv4Address(' + S(v) + ')', None),
    ('current_address.netmask', 'Netmask', I('byte'), None),
    ('current_address.dhcp_status', 'DHCPStatus', E('ERDM_DHCPStatusMode'), None)]),
 ('IPV4_DHCP_MODE', 'GET_COMMAND'): _simple('uint', 'dhcp_mode.interface_identifier', U),
 ('IPV4_STATIC_ADDRESS', 'GET_COMMAND'): _simple('uint', 'static_address.interface_identifier', U),
 ('LANGUAGE', 'GET_COMMAND_RESPONSE'): _simple('string', 'language_code', S),
 ('LANGUAGE_CAPABILITIES', 'GET_COMMAND_RESPONSE'): ('string[]', lambda fl: [
    'Assert.That(obj, Is.EqualTo(new string[] { ' + ', '.join(S(v) for v in _list(fl, 'language_code')) + ' }));']),
 ('LIST_INTERFACES', 'GET_COMMAND_RESPONSE'): ('InterfaceDescriptor[]', lambda fl: [
    'Assert.That(obj.Select(i => i.InterfaceId), Is.EqualTo(new uint[] { ' + ', '.join(str(num(v)) + 'u' for v in _list(fl, 'list_interfaces.interface_identifier')) + ' }));']),
 ('LOCK_PIN', 'GET_COMMAND_RESPONSE'): _simple('ushort', 'lock_pin.pin_code', I('ushort')),
 ('MAXIMUM_LEVEL', 'GET_COMMAND_RESPONSE'): _simple('ushort', 'maximum_level.level', I('ushort')),
 ('MINIMUM_LEVEL', 'GET_COMMAND_RESPONSE'): ('RDMMinimumLevel', [
    ('minimum_level.increasing', 'MinimumLevelIncrease', I('ushort'), None),
    ('minimum_level.decreasing', 'MinimumLevelDecrease', I('ushort'), None),
    ('minimum_level.on_below_minimum', 'OnBelowMinimum', B, None)]),
 ('MODULATION_FREQUENCY', 'GET_COMMAND_RESPONSE'): ('RDMModulationFrequency', [
    ('modulation_frequency.modulation_frequency', 'ModulationFrequencyId', I('byte'), None),
    ('modulation_frequency.number_of_modulation_frequencies', 'ModulationFrequencys', I('byte'), None)]),
 ('MODULATION_FREQUENCY_DESCRIPTION', 'GET_COMMAND'): _simple('byte', 'modulation_frequency_description.modulation_frequency', I('byte')),
 ('MODULATION_FREQUENCY_DESCRIPTION', 'GET_COMMAND_RESPONSE'): ('RDMModulationFrequencyDescription', lambda fl: [
    'Assert.That(obj.ModulationFrequencyId, Is.EqualTo((byte)' + str(num(_list(fl, 'modulation_frequency_description.modulation_frequency')[0])) + '));',
    'Assert.That(obj.Frequency, Is.EqualTo(' + str(num(_list(fl, 'modulation_frequency_description.modulation_frequency')[1])) + 'u));',
    'Assert.That(obj.Description, Is.EqualTo(' + S(first(fl, 'modulation_frequency_description.description')) + '));']),
 ('OUTPUT_RESPONSE_TIME', 'GET_COMMAND_RESPONSE'): ('RDMOutputResponseTime', [
    ('response_time.response_time', 'CurrentResponseTimeId', I('byte'), None),
    ('response_time.number_of_response_times', 'ResponseTimes', I('byte'), None)]),
 ('OUTPUT_RESPONSE_TIME_DESCRIPTION', 'GET_COMMAND'): _simple('byte', 'output_response_time_description.output_response_time', I('byte')),
 ('OUTPUT_RESPONSE_TIME_DESCRIPTION', 'GET_COMMAND_RESPONSE'): ('RDMOutputResponseTimeDescription', [
    ('output_response_time_description.output_response_time', 'OutputResponseTimeId', I('byte'), None),
    ('output_response_time_description.description', 'Description', S, None)]),
 ('PAN_INVERT', 'GET_COMMAND_RESPONSE'): _simple('bool', 'pan_invert', B),
 ('TILT_INVERT', 'GET_COMMAND_RESPONSE'): _simple('bool', 'tilt_invert', B),
 ('PAN_TILT_SWAP', 'GET_COMMAND_RESPONSE'): _simple('bool', 'tilt_swap', B),
 ('PERFORM_SELFTEST', 'GET_COMMAND_RESPONSE'): _simple('bool', 'selftest.state', B),
 ('POWER_STATE', 'SET_COMMAND'): _simple('ERDM_PowerState', 'power_state', E('ERDM_PowerState')),
 ('PRESET_PLAYBACK', 'GET_COMMAND_RESPONSE'): ('RDMPresetPlayback', [
    ('preset_playback.mode', 'Mode', I('ushort'), None),
    ('preset_playback.level', 'Level', I('byte'), None)]),
 ('REAL_TIME_CLOCK', 'GET_COMMAND_RESPONSE'): ('RDMRealTimeClock', [
    ('real_time_clock.year', 'Year', I('ushort'), None),
    ('real_time_clock.month', 'Month', I('byte'), None),
    ('real_time_clock.day', 'Day', I('byte'), None),
    ('real_time_clock.hour', 'Hour', I('byte'), None),
    ('real_time_clock.minute', 'Minute', I('byte'), None),
    ('real_time_clock.second', 'Second', I('byte'), None)]),
 ('SELF_TEST_DESCRIPTION', 'GET_COMMAND'): _simple('byte', 'selftest.nr', I('byte')),
 ('SELF_TEST_DESCRIPTION', 'GET_COMMAND_RESPONSE'): ('RDMSelfTestDescription', [
    ('selftest.nr', 'SelfTestRequester', I('byte'), None),
    ('selftest.description', 'Description', S, None)]),
 ('SERIAL_NUMBER', 'GET_COMMAND_RESPONSE'): _simple('string', 'serial_number', S),
 ('SLOT_DESCRIPTION', 'GET_COMMAND'): _simple('ushort', 'slot_nr', I('ushort')),
 ('SLOT_DESCRIPTION', 'GET_COMMAND_RESPONSE'): ('RDMSlotDescription', [
    ('slot_nr', 'SlotId', I('ushort'), None),
    ('slot_description', 'Description', S, None)]),
 ('SLOT_INFO', 'GET_COMMAND_RESPONSE'): ('RDMSlotInfo[]', lambda fl: [
    'Assert.That(obj.Select(s => s.SlotOffset), Is.EqualTo(new ushort[] { ' + ', '.join(str(num(v)) for v in _list(fl, 'slot_offset')) + ' }));',
    'Assert.That(obj.Select(s => s.SlotType), Is.EqualTo(new ERDM_SlotType[] { ' + ', '.join('(ERDM_SlotType)0x%02X' % num(v) for v in _list(fl, 'slot_type')) + ' }));',
    'Assert.That(obj.Select(s => s.SlotLabelId), Is.EqualTo(new ERDM_SlotCategory[] { ' + ', '.join('(ERDM_SlotCategory)0x%04X' % num(v) for v in _list(fl, 'slot_label_id')) + ' }));']),
 ('STATUS_ID_DESCRIPTION', 'GET_COMMAND'): _simple('ushort', 'status_id', I('ushort')),
 ('STATUS_ID_DESCRIPTION', 'GET_COMMAND_RESPONSE'): _simple('string', 'status_id.description', S),
})
IP = lambda v: 'new IPv4Address(' + S(v) + ')'
MAP.update({
 ('IPV4_STATIC_ADDRESS', 'GET_COMMAND_RESPONSE'): ('GetSetIPv4StaticAddress', [
    ('static_address.interface_identifier', 'InterfaceId', U, None),
    ('static_address.ipv4_address', 'IPAddress', IP, None),
    ('static_address.netmask', 'Netmask', I('byte'), None)]),
 ('IPV4_DHCP_MODE', 'GET_COMMAND_RESPONSE'): ('GetSetIPV4_xxx_Mode', [
    ('dhcp_mode.interface_identifier', 'InterfaceId', U, None),
    ('dhcp_mode.enabled', 'Enabled', B, None)]),
 ('INTERFACE_HARDWARE_ADDRESS_TYPE', 'GET_COMMAND_RESPONSE'): ('GetHardwareAddressResponse', [
    ('hardware_address_type1.interface_identifier', 'InterfaceId', U, None),
    ('hardware_address_type1.hardware_address', 'HardwareAddress', lambda v: 'new MACAddress(' + S(v) + ')', None)]),
 ('DNS_IPV4_NAME_SERVER', 'GET_COMMAND_RESPONSE'): ('GetSetIPv4NameServer', [
    ('dns_ipv4_name_server.index', 'NameServerIndex', I('byte'), None),
    ('dns_ipv4_name_server.address', 'IPAddress', IP, None)]),
})
def _status_messages(fl):
    sub = _list(fl, 'status_messages.sub_devices_id')
    if not sub:
        return ['Assert.That(obj, Is.Empty);']
    def ints(key):
        return ', '.join(str(num(v)) for v in _list(fl, key))
    return [
        'Assert.That(obj.Select(s => (int)s.SubDeviceId), Is.EqualTo(new int[] { ' + ints('status_messages.sub_devices_id') + ' }));',
        'Assert.That(obj.Select(s => (int)s.StatusType), Is.EqualTo(new int[] { ' + ints('status_messages.type') + ' }));',
        'Assert.That(obj.Select(s => (int)s.StatusMessage), Is.EqualTo(new int[] { ' + ints('status_messages.id') + ' }));',
        'Assert.That(obj.Select(s => (ushort)s.DataValue1), Is.EqualTo(new ushort[] { ' + ints('status_messages.data_value_1') + ' }));',
        'Assert.That(obj.Select(s => (ushort)s.DataValue2), Is.EqualTo(new ushort[] { ' + ints('status_messages.data_value_2') + ' }));']

def _slot_info(fl):
    offsets = [num(v) for v in _list(fl, 'slot_offset')]
    types = [num(v) for v in _list(fl, 'slot_type')]
    labels = iter(num(v) for v in _list(fl, 'slot_label_id'))
    primaries = iter(num(v) for v in _list(fl, 'slot_primary_offset'))
    # E1.20 Table C-1: a secondary Slot carries the offset of its primary Slot instead of a Slot Label ID
    ids = [next(labels) if t == 0 else next(primaries) for t in types]
    return [
        'Assert.That(obj.Select(s => s.SlotOffset), Is.EqualTo(new ushort[] { ' + ', '.join(map(str, offsets)) + ' }));',
        'Assert.That(obj.Select(s => s.SlotType), Is.EqualTo(new ERDM_SlotType[] { ' + ', '.join('(ERDM_SlotType)0x%02X' % t for t in types) + ' }));',
        'Assert.That(obj.Select(s => (ushort)s.SlotLabelId), Is.EqualTo(new ushort[] { ' + ', '.join('0x%04X' % i for i in ids) + ' }));']

def _modulation_frequency_description(fl):
    ids = _list(fl, 'modulation_frequency_description.modulation_frequency')
    frequency = num(ids[1])
    description = first(fl, 'modulation_frequency_description.description')
    return [
        'Assert.That(obj.ModulationFrequencyId, Is.EqualTo((byte)' + str(num(ids[0])) + '));',
        # 0xFFFFFFFF: the frequency is not declared
        'Assert.That(obj.Frequency, ' + ('Is.Null' if frequency == 0xFFFFFFFF else 'Is.EqualTo(' + str(frequency) + 'u)') + ');',
        'Assert.That(obj.Description, ' + ('Is.Null.Or.Empty' if description == '' else 'Is.EqualTo(' + S(description) + ')') + ');']

MAP.update({
 ('STATUS_MESSAGES', 'GET_COMMAND_RESPONSE'): ('RDMStatusMessage[]', _status_messages),
 ('SLOT_INFO', 'GET_COMMAND_RESPONSE'): ('RDMSlotInfo[]', _slot_info),
 ('MODULATION_FREQUENCY_DESCRIPTION', 'GET_COMMAND_RESPONSE'): ('RDMModulationFrequencyDescription', _modulation_frequency_description),
})
# REAL_TIME_CLOCK SET_COMMAND has the same layout as the response
MAP[('REAL_TIME_CLOCK', 'SET_COMMAND')] = MAP[('REAL_TIME_CLOCK', 'GET_COMMAND_RESPONSE')]
