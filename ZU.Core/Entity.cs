using System;

namespace ZU.Storage
{
    public class Entity : IEntity
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public DateTime TLBirth { get; set; }
        public DateTime TLChange { get; set; }
        public DateTime TLDeath { get; set; }
    }
}