namespace BookStoreMVC.Models
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; set; }
        public int TotalPage { get; set; }
        public int StartPage { get; set; }
        public int EndPage { get; set; }

        public IEnumerable<string> Headers { get; set; }

        public string Type { get; set; }


        public PaginatedList(IList<T> items, int count, int pageIndex, int pageSize, IEnumerable<string> headers, string type)
        {
            PageIndex = pageIndex;
            TotalPage = (int)Math.Ceiling(count / (double)pageSize);
            Headers = headers;
            Type = type;

            StartPage = PageIndex - 5;
            EndPage = PageIndex + 4;
            if (StartPage <= 0)
            {
                EndPage -= (StartPage - 1);
                StartPage = 1;
            }
            if (EndPage > TotalPage)
            {
                EndPage = TotalPage;

                if (EndPage > 10)
                {
                    StartPage = EndPage - 9;
                }
            }
            this.AddRange(items);
        }

        public static PaginatedList<T> Create(IList<T> source, int pageIndex, int pageSize, IEnumerable<string> Headers, string type)
        {
            var count = source.Count();
            var items = source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToList();

            return new PaginatedList<T>(items, count, pageIndex, pageSize, Headers, type);
        }

        public bool HasPreviousPage => PageIndex > 1;
        public bool HasNextPage => PageIndex < TotalPage;


    }
}