﻿namespace BibleTraining.Web.UI.Features.Search
{
    using System.Linq;
    using DataTables.AspNet.Core;
    using Entities;
    using Improving.Highway.Data.Scope.Repository;

    public class CoursesController
        : DataTablesSearchController<Course, IBibleTrainingDomain>
    {
        public CoursesController(IRepository<IBibleTrainingDomain> repository)
            :base(repository)
        {
        }

        protected override IQueryable<Course> DefaultSort(IQueryable<Course> queryable)
        {
            return queryable.OrderBy(x => x.Name);
        }

        protected override IQueryable<Course> SortColumn(IQueryable<Course> queryable, IColumn column)
        {
            if(column.Is(nameof(Course.Name)))
                return column.Sort.Direction == SortDirection.Descending
                    ? queryable.OrderByDescending(x => x.Name)
                    : queryable.OrderBy(x => x.Name);

            if(column.Is(nameof(Course.Description)))
                return column.Sort.Direction == SortDirection.Descending
                    ? queryable.OrderByDescending(x => x.Description)
                    : queryable.OrderBy(x => x.Description);

            return queryable;
        }

        protected override IQueryable<Course> SearchAllColumns(IQueryable<Course> queryable, ISearch search)
        {
             return queryable.Where(x =>
                x.Name.Contains(search.Value) ||
                x.Description.Contains(search.Value));
        }

        protected override IQueryable<Course> FilterColumn(IQueryable<Course> queryable, IColumn column)
        {
            if(column.Is(nameof(Course.Name)))
                return queryable.Where(x => x.Name.Contains(column.Search.Value));
            if(column.Is(nameof(Course.Description)))
                return queryable.Where(x => x.Description.Contains(column.Search.Value));
            return queryable;
        }
    }
}
