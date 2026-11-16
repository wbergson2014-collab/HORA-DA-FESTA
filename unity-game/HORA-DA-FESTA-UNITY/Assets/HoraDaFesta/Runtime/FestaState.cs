using System;

namespace HoraDaFesta
{
    [Serializable]
    public class Person
    {
        public string name;
        public string role;
        public int roses;
    }

    [Serializable]
    public class Musician
    {
        public string id;
        public string instrument;
    }

    [Serializable]
    public class Couple
    {
        public string noivo;
        public string noiva;
    }

    [Serializable]
    public class Effect
    {
        public string kind;
        public long until;
    }

    [Serializable]
    public class Focus
    {
        public string user;
        public long until;
    }

    [Serializable]
    public class FestaState
    {
        public int mode;
        public Person[] people;
        public Musician[] band;
        public Couple couple;
        public string camera;
        public Effect effect;
        public Focus focus;
    }

    [Serializable]
    public class Envelope
    {
        public string type;
        public long revision;
        public FestaState state;
        public string command;
        public CommandPayload payload;
        public string user;
        public string kind;
    }

    [Serializable]
    public class CommandPayload
    {
        public string mode;
        public float seconds;
    }
}
