using System;
using Newtonsoft.Json;

namespace Samples.CoconutWaxWallet.Scripts.Data
{
    [Serializable]
    public class AlienWorldsAssetData
    {
        [JsonProperty("img")] public string ImageKey { get; set; }
        [JsonProperty("backimg")] public string BackImageKey { get; set; }
        [JsonProperty("name")] public string Name { get; set; }
        [JsonProperty("description")] public string Description { get; set; }
        [JsonProperty("race")] public string Race { get; set; }
        [JsonProperty("shine")] public string Shine { get; set; }
        [JsonProperty("attack")] public uint Attack { get; set; }
        [JsonProperty("cardid")] public uint CardId { get; set; }
        [JsonProperty("rarity")] public string Rarity { get; set; }
        [JsonProperty("defense")] public uint Defense { get; set; }
        [JsonProperty("element")] public string Element { get; set; }
        [JsonProperty("movecost")] public uint MoveCost { get; set; }

        public override string ToString()
        {
            return
                $"{nameof(ImageKey)}: {ImageKey}, {nameof(BackImageKey)}: {BackImageKey}, {nameof(Name)}: {Name}, {nameof(Description)}: {Description}, {nameof(Race)}: {Race}, {nameof(Shine)}: {Shine}, {nameof(Attack)}: {Attack}, {nameof(CardId)}: {CardId}, {nameof(Rarity)}: {Rarity}, {nameof(Defense)}: {Defense}, {nameof(Element)}: {Element}, {nameof(MoveCost)}: {MoveCost}";
        }
    }
}