namespace MasterAPI.Application.Core
{
    public abstract class PagingParams
    {
        public int PageNumber { get; set; } = 1;
        private const int MAXPAGESIZE = 50;
        private int _pageSize = 10;

        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = (value > MAXPAGESIZE) ? MAXPAGESIZE : value;
        }
        public string? OrderBy { get; set; }
        public bool? OrderAsc { get; set; } = true;
    }
}