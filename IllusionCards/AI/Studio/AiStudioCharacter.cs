﻿using System.Collections.Immutable;
using System.Text.Json;

using IllusionCards.AI.Chara;
using IllusionCards.Cards;
using IllusionCards.FakeUnity;

using static IllusionCards.AI.Cards.AiSceneCard;
using static IllusionCards.Chara.IIllusionChara;

namespace IllusionCards.AI.Studio
{
	public record AiStudioCharacter : AiStudioObject
	{
		private CharaSex Sex { get => (CharaSex)Chara.Parameter.sex; }
		private int Index { get; init; }
		public override int Kind { get => 0; }

		public AiChara Chara { get; init; }

		public ImmutableDictionary<int, ImmutableArray<AiStudioObject>> Children { get; init; }
		public ImmutableDictionary<int, AiStudioBone> Bones { get; init; }


		// Character Status
		public ImmutableDictionary<int, TreeNode.TreeState> AccessoryGroup { get; init; }
		public ImmutableDictionary<int, TreeNode.TreeState> AccessoryNumber { get; init; }
		private ImmutableArray<byte> CumSplatterStatusBytes { get; init; }
		public enum CumSplatterState
		{
			None = 0b00,
			Half = 0b01,
			Full = 0b10
		}
		public readonly struct CumSplatterStatus
		{
			public CumSplatterState Face { get; init; }
			public CumSplatterState Chest { get; init; }
			public CumSplatterState Stomach { get; init; }
			public CumSplatterState Back { get; init; }
			public CumSplatterState Ass { get; init; }
		}
		public CumSplatterStatus CumSplatter { get; init; }
		public float NippleStiffness { get; init; }
		public float Wetness { get => Chara.Status.wetRate; }
		public float SkinGloss { get => Chara.Status.skinTuyaRate; }
		public bool CharaIsMonochrome { get; init; }
		public Color MonochromeColor { get; init; }
		public bool PenisIsVisible { get; init; } // Illusion calls it "son"
		public float PenisLength { get; init; }

		// Kinematics
		public KinematicMode KinematicModeStatus { get; init; } // FK + IK is a mod
		public bool FKEnabled { get; init; }
		private ImmutableArray<bool> ActiveFKArray { get; init; } = new bool[7] { false, false, false, false, false, false, false, }.ToImmutableArray();
		public readonly struct ActiveFKStruct
		{
			public bool Hair { get; init; }
			public bool Neck { get; init; }
			public bool Breast { get; init; }
			public bool Body { get; init; }
			public bool RightHand { get; init; }
			public bool LeftHand { get; init; }
			public bool Skirt { get; init; }
		}
		public ActiveFKStruct ActiveFK { get; init; }
		public ImmutableDictionary<int, AiIKTarget> IKTargets { get; init; }
		public bool IKEnabled { get; init; }
		private ImmutableArray<bool> ActiveIKArray { get; init; } = new bool[5] { false, false, false, false, false, }.ToImmutableArray();
		public readonly struct ActiveIKStruct
		{
			public bool Body { get; init; }
			public bool RightLeg { get; init; }
			public bool LeftLeg { get; init; }
			public bool RightArm { get; init; }
			public bool LeftArm { get; init; }
		}
		public ActiveIKStruct ActiveIK { get; init; }
		public AiStudioLookAtTarget LookAtTarget { get; init; }
		public enum EyeLookType
		{
			NO_LOOK,
			TARGET,
			AWAY,
			FORWARD,
			CONTROL
		}
		public EyeLookType EyePosition { get => (EyeLookType)Chara.Status.eyesLookPtn; }
		public ImmutableArray<Quarternion> EyeAngles { get; init; }
		//public enum NECK_LOOK_TYPE
		//{
		//	NO_LOOK,
		//	TARGET,
		//	AWAY,
		//	FORWARD,
		//	FIX,
		//	CONTROL
		//}
		public enum NeckLookType // NECK_LOOK_TYPE_VER2
		{
			ANIMATION,
			TARGET,
			AWAY,
			FORWARD,
			FIX,
			CONTROL
		}
		public NeckLookType NeckPosition { get; init; }
		public ImmutableArray<Quarternion> NeckAngles { get; init; }
		public float MouthOpen { get; init; }
		public bool LipSync { get; init; } = true;
		private ImmutableArray<int> HandPosition { get; init; } = new int[2] { 0, 0 }.ToImmutableArray();
		public int LeftHandPosition { get => HandPosition[0]; }
		public int RightHandPosition { get => HandPosition[1]; }

		// Animation
		public AnimeInfo AnimeInfoStatus { get; init; }
		public float AnimeSpeed { get; init; }
		public float AnimePattern { get; init; }
		public ImmutableArray<float> AnimeOptionParams { get; init; }
		public bool AnimeShowAttachedItem { get; init; }
		public bool AnimeIsForceLooped { get; init; }
		public float AnimeNormalizedTime { get; init; }
		public AiVoiceControl VoiceControl { get; init; }
		// Joint correction?
		private ImmutableArray<bool> JCArray { get; init; }
		public readonly struct JointCorrectionStruct
		{
			public bool RightShoulder { get; init; }
			public bool LeftShoulder { get; init; }
			public bool RightLeg { get; init; }
			public bool LeftLeg { get; init; }
			public bool RightFS { get; init; }
			public bool LeftFS { get; init; }
			public bool RightFat { get; init; }
			public bool LeftFat { get; init; }
		}
		public JointCorrectionStruct JointCorrection { get; init; }


		public AiStudioCharacter(BinaryReader binaryReader) : base(binaryReader)
		{
			// TODO: verify mapping of sex - I think female is 1 here but 0 in AiChara
			int _sex = binaryReader.ReadInt32();
			Chara = new(binaryReader);
			int _count = binaryReader.ReadInt32();
			Dictionary<int, AiStudioBone> _bones = new();
			for (int i = 0; i < _count; i++)
			{
				int key = binaryReader.ReadInt32();
				_bones[key] = new AiStudioBone(binaryReader);
			}
			Bones = _bones.ToImmutableDictionary();

			_count = binaryReader.ReadInt32();
			Dictionary<int, AiIKTarget> _ikTargets = new();
			for (int i = 0; i < _count; i++)
			{
				int key = binaryReader.ReadInt32();
				_ikTargets[key] = new AiIKTarget(binaryReader);
			}
			IKTargets = _ikTargets.ToImmutableDictionary();

			_count = binaryReader.ReadInt32();
			Dictionary<int, ImmutableArray<AiStudioObject>> _children = new();
			for (int i = 0; i < _count; i++)
			{
				int _key = binaryReader.ReadInt32();
				ImmutableDictionary<int, AiStudioObject> __childObjects = GetStudioObjects(binaryReader);
				var _childObjectsBuilder = ImmutableArray.CreateBuilder<AiStudioObject>();
				_childObjectsBuilder.AddRange(__childObjects.Values);
				_children.Add(_key, _childObjectsBuilder.ToImmutable());
			}
			Children = _children.ToImmutableDictionary();

			KinematicModeStatus = (KinematicMode)binaryReader.ReadInt32();
			AnimeInfoStatus = new(binaryReader);
			int[] _hPos = new int[2];
			_hPos[0] = binaryReader.ReadInt32();
			_hPos[1] = binaryReader.ReadInt32();
			HandPosition = _hPos.ToImmutableArray();
			NippleStiffness = binaryReader.ReadSingle();
			CumSplatterStatusBytes = binaryReader.ReadBytes(5).ToImmutableArray();
			CumSplatter = new()
			{
				Face = (CumSplatterState)CumSplatterStatusBytes[0],
				Chest = (CumSplatterState)CumSplatterStatusBytes[1],
				Stomach = (CumSplatterState)CumSplatterStatusBytes[2],
				Back = (CumSplatterState)CumSplatterStatusBytes[3],
				Ass = (CumSplatterState)CumSplatterStatusBytes[4]
			};
			MouthOpen = binaryReader.ReadSingle();
			LipSync = binaryReader.ReadBoolean();
			LookAtTarget = new(binaryReader);
			IKEnabled = binaryReader.ReadBoolean();
			bool[] _activeIK = new bool[5];
			for (int i = 0; i < 5; i++)
			{
				_activeIK[i] = binaryReader.ReadBoolean();
			}
			ActiveIKArray = _activeIK.ToImmutableArray();
			// Note for later: these indices correspond to the BoneGroup enum (1 << i)
			ActiveIK = new()
			{
				Body = ActiveIKArray[0],
				RightLeg = ActiveIKArray[1],
				LeftLeg = ActiveIKArray[2],
				RightArm = ActiveIKArray[3],
				LeftArm = ActiveIKArray[4]
			};
			FKEnabled = binaryReader.ReadBoolean();
			bool[] _activeFK = new bool[7];
			for (int i = 0; i < 7; i++)
			{
				_activeFK[i] = binaryReader.ReadBoolean();
			}
			ActiveFKArray = _activeFK.ToImmutableArray();
			// FKCtrl.parts
			ActiveFK = new()
			{
				Hair = ActiveFKArray[0],
				Neck = ActiveFKArray[1],
				Breast = ActiveFKArray[2],
				Body = ActiveFKArray[3],
				RightHand = ActiveFKArray[4],
				LeftHand = ActiveFKArray[5],
				Skirt = ActiveFKArray[6]
			};
			bool[] _jc = new bool[8];
			for (int i = 0; i < 8; i++)
			{
				_jc[i] = binaryReader.ReadBoolean();
			}
			JCArray = _jc.ToImmutableArray();
			JointCorrection = new()
			{
				RightShoulder = JCArray[0],
				LeftShoulder = JCArray[1],
				RightLeg = JCArray[2],
				LeftLeg = JCArray[3],
				RightFS = JCArray[4],
				LeftFS = JCArray[5],
				RightFat = JCArray[6],
				LeftFat = JCArray[7],
			};
			AnimeSpeed = binaryReader.ReadSingle();
			AnimePattern = binaryReader.ReadSingle();
			AnimeShowAttachedItem = binaryReader.ReadBoolean();
			AnimeIsForceLooped = binaryReader.ReadBoolean();
			VoiceControl = new(binaryReader);
			PenisIsVisible = binaryReader.ReadBoolean();
			PenisLength = binaryReader.ReadSingle();
			CharaIsMonochrome = binaryReader.ReadBoolean();
			_count = binaryReader.Read7BitEncodedInt();
			Utf8JsonReader _colorJsonReader = new(binaryReader.ReadBytes(_count), new() { MaxDepth = 64 });
			float? _r = null;
			float? _g = null;
			float? _b = null;
			float? _a = null;
			while (_colorJsonReader.Read())
			{
				Console.WriteLine(_colorJsonReader.TokenType);
				string? _prop;
				switch (_colorJsonReader.TokenType)
				{
					case JsonTokenType.PropertyName:
						_prop = _colorJsonReader.GetString();
						_colorJsonReader.Read();
						switch (_prop)
						{
							case "r":
								_r = _colorJsonReader.GetSingle();
								break;
							case "g":
								_g = _colorJsonReader.GetSingle();
								break;
							case "b":
								_b = _colorJsonReader.GetSingle();
								break;
							case "a":
								_a = _colorJsonReader.GetSingle();
								break;
						}
						break;
					case JsonTokenType.StartObject:
					case JsonTokenType.EndObject:
						break;
					default:
						throw new InvalidCardException("Could not read color JSON.");
				};
			}
			if (_r is null || _g is null || _b is null || _a is null)
				throw new InvalidCardException("Could not read color JSON.");
			MonochromeColor = new() { r = (float)_r, g = (float)_g, b = (float)_b, a = (float)_a };
			float[] _animeOptions = new float[2] { 0f, 0f };
			_animeOptions[0] = binaryReader.ReadSingle();
			_animeOptions[1] = binaryReader.ReadSingle();
			AnimeOptionParams = _animeOptions.ToImmutableArray();
			_ = binaryReader.ReadInt32();
			NeckPosition = (NeckLookType)binaryReader.ReadInt32();
			var _neckAngleBuilder = ImmutableArray.CreateBuilder<Quarternion>();
			_count = binaryReader.ReadInt32();
			for (int i = 0; i < _count; i++)
				_neckAngleBuilder.Add(new(binaryReader));
			NeckAngles = _neckAngleBuilder.ToImmutable();
			_ = binaryReader.ReadInt32();
			Quarternion[] _eyeAngles = new Quarternion[2];
			_eyeAngles[0] = new(binaryReader);
			_eyeAngles[1] = new(binaryReader);
			EyeAngles = _eyeAngles.ToImmutableArray();
			AnimeNormalizedTime = binaryReader.ReadSingle();
			var _acsGroup = ImmutableDictionary.CreateBuilder<int, TreeNode.TreeState>();
			_count = binaryReader.ReadInt32();
			for (int i = 0; i < _count; i++)
			{
				int _key = binaryReader.ReadInt32();
				_acsGroup[_key] = (TreeNode.TreeState)binaryReader.ReadInt32();
			}
			AccessoryGroup = _acsGroup.ToImmutable();
			var _acsNum = ImmutableDictionary.CreateBuilder<int, TreeNode.TreeState>();
			_count = binaryReader.ReadInt32();
			for (int i = 0; i < _count; i++)
			{
				int _key = binaryReader.ReadInt32();
				_acsNum[_key] = (TreeNode.TreeState)binaryReader.ReadInt32();
			}
			AccessoryNumber = _acsNum.ToImmutable();
		}
		public enum IKTargetEN
		{
			Body,
			LeftShoulder,
			LeftArmChain,
			LeftHand,
			RightShoulder,
			RightArmChain,
			RightHand,
			LeftThigh,
			LeftLegChain,
			LeftFoot,
			RightThigh,
			RightLegChain,
			RightFoot
		}
		public enum KinematicMode { None, IK, FK }
		public readonly struct AnimeInfo
		{
			public int Group { get; init; } = -1;
			public int Category { get; init; } = -1;
			public int Number { get; init; } = -1;
			public bool Exists { get => Group != -1 && Category != -1 && Number != -1; }

			public AnimeInfo(BinaryReader binaryReader)
			{
				Group = binaryReader.ReadInt32();
				Category = binaryReader.ReadInt32();
				Number = binaryReader.ReadInt32();
			}
		}
	}
}
