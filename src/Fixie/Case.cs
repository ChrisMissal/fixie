﻿namespace Fixie
{
    public interface Case
    {
        string Name { get; }
        void Execute();
    }
}