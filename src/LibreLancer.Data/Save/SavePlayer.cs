﻿// MIT License - Copyright (c) Callum McGing
// This file is subject to the terms and conditions defined in
// LICENSE, which is part of this source code package

using System;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Collections.Generic;
using System.Text;
using LibreLancer.Ini;
    
namespace LibreLancer.Data.Save
{
    public class PlayerEquipment
    {
        public uint EquipHash;
        public string EquipName;
        public string Hardpoint;
        public string Unknown; //Either health or count, not sure
        public PlayerEquipment() { }
        public PlayerEquipment(Entry e)
        {
            var s = e[0].ToString();
            if (!uint.TryParse(s, out EquipHash)) EquipName = s;
            if (e.Count < 2) return;
            //Extra
            Hardpoint = e[1].ToString();
            if(e.Count > 2) Unknown = e[2].ToString();
        }
    }

    public class PlayerCargo
    {
        public uint CargoHash;
        public string CargoName;
        public int Count;
        //Some unknowns here
        public PlayerCargo() { }
        public PlayerCargo(Entry e)
        {
            var s = e[0].ToString();
            if (!uint.TryParse(s, out CargoHash)) CargoName = s;
            Count = e[1].ToInt32();
        }
    }


    public class SavePlayer
    {
        [Entry("descrip_strid")] public int DescripStrid;

        [Entry("description")] public string Description;

        //HandleEntry (tstamp)
        public DateTime? TimeStamp;

        //HandleEntry (name)
        public string Name;
        [Entry("rank")] public string Rank;

        [Entry("money")] public long Money;

        [Entry("num_kills")] public int NumKills;
        [Entry("num_misn_successes")] public int NumMissionSuccesses;
        [Entry("num_misn_failures")] public int NumMissionFailures;

        //HandleEntry(house)
        public List<SaveRep> House = new List<SaveRep>();

        [Entry("voice")] public string Voice;
        [Entry("costume")] public string Costume;
        [Entry("com_costume")] public string ComCostume;
        [Entry("com_body")] public int ComBody;
        [Entry("com_head")] public int ComHead;
        [Entry("com_lefthand")] public int ComLeftHand;
        [Entry("com_righthand")] public int ComRightHand;
        [Entry("body")] public int Body;
        [Entry("head")] public int Head;
        [Entry("lefthand")] public int LeftHand;
        [Entry("righthand")] public int RightHand;

        [Entry("system")] public string System;
        [Entry("base")] public string Base;
        [Entry("pos")] public Vector3 Position;
        [Entry("rotate")] public Vector3 Rotate;

        public int ShipArchetypeCrc;
        public string ShipArchetype;

        //HandleEntry(equip)
        public List<PlayerEquipment> Equip = new List<PlayerEquipment>();

        //HandleEntry(cargo)
        public List<PlayerCargo> Cargo = new List<PlayerCargo>();
        //HandleEntry(visit)


        [Entry("interface")] public int Interface;


        [Entry("house", Multiline = true)]
        void HandleHouse(Entry e) => House.Add(new SaveRep(e));

        [Entry("log")]
        [Entry("visit")]
        void Noop(Entry e)
        {
        }

        [Entry("tstamp")]
        void HandleTimestamp(Entry e) => TimeStamp = DateTime.FromFileTime(e[0].ToInt64() << 32 | e[1].ToInt64());

        [Entry("name")]
        void HandleName(Entry e)
        {
            try
            {
                var bytes = SplitInGroups(e[0].ToString(), 2).Select(x => byte.Parse(x, NumberStyles.HexNumber)).ToArray();
                Name = Encoding.BigEndianUnicode.GetString(bytes);
            }
            catch (Exception)
            {
                Name = e[0].ToString();
            }
        }

        [Entry("equip", Multiline = true)]
        void HandleEquip(Entry e) => Equip.Add(new PlayerEquipment(e));

        [Entry("cargo", Multiline = true)]
        void HandleCargo(Entry e) => Cargo.Add(new PlayerCargo(e));

        [Entry("ship_archetype")]
        void HandleShipArchetype(Entry e)
        {
            ShipArchetypeCrc = e[0].ToInt32();
            if(ShipArchetypeCrc == -1)
                ShipArchetype = e[0].ToString();
        }
        static IEnumerable<string> SplitInGroups(string original, int size)
        {
            var p = 0;
            var l = original.Length;
            while (l - p > size)
            {
                yield return original.Substring(p, size);
                p += size;
            }
            var s = original.Substring(p);
            if (!string.IsNullOrWhiteSpace(s) && !string.IsNullOrEmpty(s)) yield return s;
        }
    }
}
