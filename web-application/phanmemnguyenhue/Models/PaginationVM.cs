using System;

namespace phanmemnguyenhue.Models
{
    public class PaginationVM
    {
        public int PageIndex { get; set; }

        public int RowCount { get; set; }

        public int PageSize { get; set; }

        public int LinkLimit { get; set; }

        public PaginationVM(int pageIndex, int pageSize, int rowCount, int linkLimit = 10)
        {
            PageIndex = pageIndex <= 0 ? 1 : pageIndex;
            PageSize = pageSize;
            RowCount = rowCount;
            LinkLimit = linkLimit;
            PageStart = 1;
            TotalPage = RowCount > 0 && PageSize > 0 ? (int)Math.Ceiling(RowCount / (double)PageSize) : 0;
            PageEnd = TotalPage;
            if (TotalPage > LinkLimit)
            {
                var middle = (int)Math.Ceiling(LinkLimit / 2d) - 1;
                var below = PageIndex - middle;
                var above = PageIndex + middle;
                if (below < 2)
                {
                    above = LinkLimit;
                    below = 1;
                }
                else if (above > (TotalPage - 2))
                {
                    above = TotalPage;
                    below = TotalPage - LinkLimit + 1;
                }
                PageStart = below;
                PageEnd = above;
            }
        }

        public int PageStart { get; set; }

        public int PageEnd { get; set; }

        public int TotalPage { get; set; }
    }
}