using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Scraper.GW2DB
{
    public class SlimItem
    {
        public SlimItem()
        {

        }

        //"ID": 607231, 
        //"ExternalID": 59987, 
        //"DataID": 8678, 
        //"Name": "Rock", 
        //"Rarity": 2, 
        //"Value": 0, 
        //"Defense": 0, 
        //"MinPower": 0, 
        //"MaxPower": 0, 
        //"Type": 4, 
        //"Description": "Doubleclick to use rock", 
        //"Level": 24, 
        //"RequiredLevel": 22, 
        //"ConsumableType": 1, 
        //"Stats": [ ]

        int _id;
        public int Id
        {
            get { return _id; }
            set { _id = value; }
        }

        int _externalId;
        public int ExternalId
        {
            get { return _externalId; }
            set { _externalId = value; }
        }

        int _dataId;
        public int DataId
        {
            get { return _dataId; }
            set { _dataId = value; }
        }

        string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        int _rarity;
        public int Rarity
        {
            get { return _rarity; }
            set { _rarity = value; }
        }

        int _value;
        public int Value
        {
            get { return _value; }
            set { _value = value; }
        }

        int _defense;
        public int Defense
        {
            get { return _defense; }
            set { _defense = value; }
        }

        int _minPower;
        public int MinPower
        {
            get { return _minPower; }
            set { _minPower = value; }
        }

        int _maxPower;
        public int MaxPower
        {
            get { return _maxPower; }
            set { _maxPower = value; }
        }

        int _type;
        public int Type
        {
            get { return _type; }
            set { _type = value; }
        }

        String _description;
        public String Description
        {
            get { return _description; }
            set { _description = value; }
        }

        int _level;
        public int Level
        {
            get { return _level; }
            set { _level = value; }
        }

        int _requiredLevel;
        public int RequiredLevel
        {
            get { return _requiredLevel; }
            set { _requiredLevel = value; }
        }

        int _consumableType;
        public int ConsumableType
        {
            get { return _consumableType; }
            set { _consumableType = value; }
        }
    }
}
