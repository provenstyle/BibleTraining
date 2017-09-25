namespace $ApplicationName$.Web.UI.Features.Search
{
    using System.Linq;
    using DataTables.AspNet.Core;
    using Entities;
    using Improving.Highway.Data.Scope.Repository;

    public class $EntityPlural$Controller
        : DataTablesSearchController<$Entity$, IBibleTrainingDomain>
    {
        public $EntityPlural$Controller(IRepository<IBibleTrainingDomain> repository)
            :base(repository)
        {
        }

        protected override IQueryable<$Entity$> DefaultSort(IQueryable<$Entity$> queryable)
        {
            return queryable.OrderBy(x => x.Name);
        }

        protected override IQueryable<$Entity$> SortColumn(IQueryable<$Entity$> queryable, IColumn column)
        {
            if(column.Is(nameof($Entity$.Name)))
                return column.Sort.Direction == SortDirection.Descending
                    ? queryable.OrderByDescending(x => x.Name)
                    : queryable.OrderBy(x => x.Name);

            return queryable;
        }

        protected override IQueryable<$Entity$> SearchAllColumns(IQueryable<$Entity$> queryable, ISearch search)
        {
             return queryable.Where(x =>
                x.Name.Contains(search.Value));
        }

        protected override IQueryable<$Entity$> FilterColumn(IQueryable<$Entity$> queryable, IColumn column)
        {
            if(column.Is(nameof($Entity$.Name)))
                return queryable.Where(x => x.Name.Contains(column.Search.Value));
            return queryable;
        }
    }
}
