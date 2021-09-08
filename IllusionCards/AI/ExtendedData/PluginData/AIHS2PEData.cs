namespace IllusionCards.AI.ExtendedData.PluginData;

public record AIHS2PEData : ExtendedPluginData
{
	public const string DataKeyAI = DefinitionMetadata.DataKeyAI;
	public const string DataKeyHS2 = DefinitionMetadata.DataKeyHS2;
	public readonly struct DefinitionMetadata
	{
		public const string PluginGUID = "com.joan6694.illusionplugins.poseeditor";
		public const string DataKeyAI = "aipe";
		public const string DataKeyHS2 = "hs2pe";
		public static readonly Version PluginVersion = new("2.12.0");
		public const string RepoURL = "https://bitbucket.org/Joan6694/hsplugins";
		public const string ClassDefinitionsURL = "https://bitbucket.org/Joan6694/hsplugins/src/master/HSPE/CharaPoseController.cs";
		public const string? License = null;
	}
	public static readonly DefinitionMetadata Metadata = new();
	public override string Name => "AIPE/HS2PE";
	public override Type DataType { get; } = typeof(XmlDocument);
	public XmlDocument Data { get; init; }

	public AIHS2PEData(int version, Dictionary<object, object> dataDict) : base(version, dataDict)
	{
		string _rawData = (string)dataDict["characterInfo"];
		XmlDocument _xmlDoc = new();
		_xmlDoc.LoadXml(_rawData);
		Data = _xmlDoc;
	}
}
