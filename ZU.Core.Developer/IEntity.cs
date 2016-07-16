using System;

namespace ZU.Storage
{
    public interface IEntity
    {
        string Id { get; set; }
        string Title { get; set; }
        DateTime TLBirth { get; set; }
        DateTime TLChange { get; set; }
        DateTime TLDeath { get; set; }
    }
}