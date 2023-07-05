using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectBoard.Data.Abstractions
{
    public class PaginationResult<T>
    {
        public List<T> Items { get; set; }

        public string? NextPageKey { get; set; }

        public PaginationResult(List<T> items, string? nextPageKey)
        {
            Items = items;
            NextPageKey = nextPageKey;
        }
    }
}
