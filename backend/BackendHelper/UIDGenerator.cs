﻿namespace BackendHelper;

public class UIDGenerator
{
    public UIDGenerator(UInt64 start)
    {
        _cid = start;
    }

    UInt64 GetNextID() => _cid++;
    
    private UInt64 _cid;
}