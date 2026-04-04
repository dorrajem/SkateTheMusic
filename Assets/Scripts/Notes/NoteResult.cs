using System;

public enum HitGrade { Perfect, Good, Miss }

[Serializable]
public class NoteResult
{
    public string   noteName;
    public bool     wasHit;
    public HitGrade grade;
}
