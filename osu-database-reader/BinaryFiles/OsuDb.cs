﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using osu_database_reader.Components.Beatmaps;
using osu_database_reader.IO;

namespace osu_database_reader.BinaryFiles
{
    public class OsuDb
    {
        public int OsuVersion;
        public int FolderCount;
        public bool AccountUnlocked;
        public DateTime AccountUnlockDate;
        public string AccountName;
        public List<BeatmapEntry> Beatmaps;
        public PlayerRank AccountRank;

        public static OsuDb Read(string path) {
            OsuDb db = new OsuDb();
            using (CustomBinaryReader r = new CustomBinaryReader(File.OpenRead(path))) {
                db.OsuVersion = r.ReadInt32();
                db.FolderCount = r.ReadInt32();
                db.AccountUnlocked = r.ReadBoolean();
                db.AccountUnlockDate = r.ReadDateTime();
                db.AccountName = r.ReadString();

                db.Beatmaps = new List<BeatmapEntry>();
                int length = r.ReadInt32();
                for (int i = 0; i < length; i++) {
                    int currentIndex = (int)r.BaseStream.Position;
                    int entryLength = r.ReadInt32();

                    db.Beatmaps.Add(BeatmapEntry.ReadFromReader(r, false, db.OsuVersion));

                    if (r.BaseStream.Position != currentIndex + entryLength + 4) {
                        Debug.Fail($"Length doesn't match, {r.BaseStream.Position} instead of expected {currentIndex + entryLength + 4}");
                    }
                }
                db.AccountRank = (PlayerRank)r.ReadByte();
            }
            return db;
        }
    }
}
