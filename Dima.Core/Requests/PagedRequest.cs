﻿namespace Dima.Core.Requests
{
    public abstract class PagedRequest : Request
    {
        public int PageNumber { get; set; } = 1;

        public int PageSize { get; set; } = 25;
    }
}
