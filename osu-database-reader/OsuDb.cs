﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace osu_database_reader
{
    public class OsuDb
    {
        public int OsuVersion;
        public int FolderCount;
        public bool AccountUnlocked;
        public DateTime AccountUnlockDate;
        public string PlayerName;
        public int AmountOfBeatmaps => Beatmaps.Count;
        public List<BeatmapEntry> Beatmaps;
        public int UserRank;  //TODO: make enum, contains supporter and stuff (playerrank)

        public static OsuDb Read(string path) {
            OsuDb db = new OsuDb();
            using (CustomReader r = new CustomReader(File.OpenRead(path))) {
                db.OsuVersion = r.ReadInt32();
                db.FolderCount = r.ReadInt32();
                db.AccountUnlocked = r.ReadBoolean();
                db.AccountUnlockDate = r.ReadDateTime();
                db.PlayerName = r.ReadString();

                db.Beatmaps = new List<BeatmapEntry>();
                int length = r.ReadInt32();
                for (int i = 0; i < length; i++) {
                    int currentIndex = (int)r.BaseStream.Position;
                    int entryLength = r.ReadInt32();

                    var entry = BeatmapEntry.ReadFromReader(r, false, db.OsuVersion);

                    db.Beatmaps.Add(entry);
                    if (r.BaseStream.Position != currentIndex + entryLength + 4) {
                        Debug.Fail($"Length doesn't match, {r.BaseStream.Position} instead of expected {currentIndex + entryLength + 4}");
                    }
                }
                db.UserRank = r.ReadByte();   //TODO: cast as rank
            }
            return db;
        }
    }
}