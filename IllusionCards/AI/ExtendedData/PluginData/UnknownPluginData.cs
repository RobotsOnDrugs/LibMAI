﻿namespace IllusionCards.AI.ExtendedData.PluginData;

public record UnknownPluginData : ExtendedPluginData
{
	public override string Name => "";
	public new Type? DataType { get; init; } = null;
	public UnknownPluginData(int version, Dictionary<object, object> dataDict) : base(version, dataDict) => RawData = dataDict;
}
