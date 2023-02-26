﻿global using System.Collections.Immutable;
global using System.Diagnostics.CodeAnalysis;
global using System.Numerics;
global using System.Reflection;
global using System.Text;
global using System.Text.Json;
global using System.Xml;

global using IllusionCards.AI.Chara;
global using IllusionCards.AI.Chara.Friendly;
global using IllusionCards.AI.Chara.Raw;
global using IllusionCards.AI.Plugins;
global using IllusionCards.AI.Mods;
global using IllusionCards.AI.Scene;
global using IllusionCards.Cards;
global using IllusionCards.Chara;
global using IllusionCards.FakeUnity;
global using IllusionCards.Illusion;
global using IllusionCards.Util;

global using MessagePack;
global using MessagePack.Formatters;
global using MessagePack.Resolvers;

global using NLog;

global using static System.Math;
global using static IllusionCards.AI.Cards.AiCharaCardDefinitions;
global using static IllusionCards.AI.Cards.AiSceneCard;
global using static IllusionCards.AI.Chara.AiCharaType;
global using static IllusionCards.AI.Chara.Friendly.AiFriendlyCharaDataConverters;
global using static IllusionCards.AI.Chara.Friendly.AiFriendlyNameLookup;
global using static IllusionCards.AI.Scene.SceneHelpers;
global using static IllusionCards.Cards.IllusionCard;
global using static IllusionCards.Chara.IIllusionChara;
global using static IllusionCards.Util.MathTypesResolver;
