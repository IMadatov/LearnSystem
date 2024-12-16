using BaseCrud.Abstractions.Entities;
using BaseCrud.Expressions;
using BaseCrud.Expressions.Filter;
using System.Linq.Expressions;

namespace LearnSystem.Models;

// Mnau custom filter uchun logika, sen endi custom sorting uchun jaza alung karak bo'ladi.
//public class SubjectExpressions : IFilterExpression<Subject>
//{
//    public Func<FilterExpressions<Subject>, FilterExpressions<Subject>> FilterExpressions =>
//        expressions => expressions.ForProperty(x => x.Name,
//            nameBuilder => nameBuilder.
//            HasFilter((subject, filterValue)
//                => subject.Name!.StartsWith(filterValue!), when: ExpressionConstraintsEnum.StartsWith));
//}

// Programmist ISortingExpression<Subject> mnani implement qilsa programma o'zi bilib ishliyviruvi karak
public class SubjectExpressions : IFilterExpression<Subject>, ISortingExpression<Subject>
{
    public Func<FilterExpressions<Subject>, FilterExpressions<Subject>> FilterExpressions =>
        expressions => expressions.ForProperty(x => x.Name,
            nameBuilder => nameBuilder.
            HasFilter((subject, filterValue)
                => subject.Name!.StartsWith(filterValue!), when: ExpressionConstraintsEnum.StartsWith));

    public Func<SortingExpressions<Subject>, SortingExpressions<Subject>> SortingExpression =>
        expressions => expressions;
            //.ForProperty(
            //    propertySelector: subject => subject.User,
            //    sortingExpression: subject => subject.User.LastName
            //)
            //.ForProperty(
            //    propertySelector: subject => subject.Name,
            //    sortingExpression: subject => subject.Name
            //)
            //.ForKey(
            //    sortingKey: "user.firstname",
            //    sortingExpression: subject => subject.User.FirstName
            //);
}

public interface ISortingExpression<T>
{
}

public class SortingExpressions<TEntity>
{
    public SortingExpressions<TEntity> ForProperty<TProperty>(
        Expression<Func<TEntity, TProperty>> propertySelector,
        Expression<Func<TEntity, object>> sortingExpression)
    {
        throw new NotImplementedException();
    }

    internal SortingExpressions<Subject> ForKey(string sortingKey, Func<Subject, object> sortingExpression)
    {
        throw new NotImplementedException();
    }
}

public class Subject : EntityBase
{

    public string? Name { get; set; }
    
    public ICollection<Journal> Journals { get; set; } = new List<Journal>();

    public ICollection<Teacher> Teachers { get; set; } = new List<Teacher>();
}
