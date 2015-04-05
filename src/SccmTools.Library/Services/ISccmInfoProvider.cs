﻿namespace SccmTools.Library.Services
{
    public interface ISccmInfoProvider
    {
        string GetSiteId();

        string GetSiteCode();

        string GetSiteServer();

        string GetAuthoringScopeId();
        string GetScopeId();
    }
}