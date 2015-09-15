using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace TradingPostDatabase
{
    public class Attribute
    {

        [JsonProperty("attribute")]
        public string AttributeName { get; set; }

        [JsonProperty("modifier")]
        public int Modifier { get; set; }
    }

    public class InfixUpgrade
    {

        [JsonProperty("attributes")]
        public IList<Attribute> Attributes { get; set; }
    }

    public class Details
    {

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("weight_class")]
        public string WeightClass { get; set; }

        [JsonProperty("defense")]
        public int Defense { get; set; }

        [JsonProperty("infusion_slots")]
        public IList<object> InfusionSlots { get; set; }

        [JsonProperty("infix_upgrade")]
        public InfixUpgrade InfixUpgrade { get; set; }

        [JsonProperty("secondary_suffix_item_id")]
        public string SecondarySuffixItemId { get; set; }
    }

    public class Item
    {

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("level")]
        public int Level { get; set; }

        [JsonProperty("rarity")]
        public string Rarity { get; set; }

        [JsonProperty("vendor_value")]
        public int VendorValue { get; set; }

        [JsonProperty("default_skin")]
        public int DefaultSkin { get; set; }

        [JsonProperty("game_types")]
        public IList<string> GameTypes { get; set; }

        [JsonProperty("flags")]
        public IList<string> Flags { get; set; }

        [JsonProperty("restrictions")]
        public IList<object> Restrictions { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("icon")]
        public string Icon { get; set; }

        [JsonProperty("details")]
        public Details Details { get; set; }
    }
}

