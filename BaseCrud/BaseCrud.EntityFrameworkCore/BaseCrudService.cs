﻿using BaseCrud.General.Entities;
using Microsoft.EntityFrameworkCore.Query;
using ServiceStatusResult;

namespace BaseCrud.EntityFrameworkCore;

public abstract partial class BaseCrudService<TEntity, TDto, TDtoFull, TKey>
    : ICrudService<TEntity, TDto, TDtoFull, TKey>, IDisposable
    where TKey : struct, IEquatable<TKey>
    where TEntity : class, IEntity<TKey>
    where TDto : class, IDataTransferObject<TEntity, TKey>
    where TDtoFull : class, IDataTransferObject<TEntity, TKey>
{
    public virtual async Task<ServiceResultBase<QueryResult<TDto>>> GetAllAsync(
        IDataTableMetaData dataTableMeta,
        IUserProfile userProfile,
        Func<IQueryable<TEntity>, IUserProfile, Task<IQueryable<TEntity>>>? customAction = null,
        CancellationToken cancellationToken = default)
    {
        var (totalCount, data) = await HandleGetAllQueryAsync(dataTableMeta, userProfile, cancellationToken, customAction);

        var result = new QueryResult<TDto>
        {
            TotalItems = totalCount,
            Items = data
        };

        return new OkServiceResult<QueryResult<TDto>>(result);
    }

    public virtual async Task<ServiceResultBase<IAsyncEnumerable<TEntity>>> GetEntityListAsync(
        IUserProfile userProfile,
        Func<IQueryable<TEntity>, IUserProfile, Task<IQueryable<TEntity>>>? customAction = null,
        CancellationToken cancellationToken = default)
    {
        var query = QueryableOfUntrackedActive;

        if (customAction != null)
            query = await customAction(query, userProfile);

        return new OkServiceResult<IAsyncEnumerable<TEntity>>(query.AsAsyncEnumerable());
    }

    public virtual async Task<ServiceResultBase<IAsyncEnumerable<TDto>>> GetListAsync(
        IUserProfile userProfile,
        Func<IQueryable<TEntity>, IUserProfile, Task<IQueryable<TEntity>>>? customAction = null,
        CancellationToken cancellationToken = default)
    {
        var query = QueryableOfUntrackedActive;

        if (customAction != null)
            query = await customAction(query, userProfile);

        var queryableOfSelected = HandleSelection(query);


        return new OkServiceResult<IAsyncEnumerable<TDto>>(queryableOfSelected.AsAsyncEnumerable());
    }

    public virtual async Task<ServiceResultBase<IAsyncEnumerable<TDtoFull>>> GetFullEntityListAsync(
        IUserProfile userProfile,
        Func<IQueryable<TEntity>, IUserProfile, Task<IQueryable<TEntity>>>? customAction = null,
        CancellationToken cancellationToken = default)
    {
        var query = QueryableOfUntrackedActive;

        if (customAction != null)
            query = await customAction(query, userProfile);

        var result = Mapper.ProjectTo<TDtoFull>(query).AsAsyncEnumerable();

        //var data = await query.ToListAsync(cancellationToken);

        //var mapped = MapDataToFullDtoList(data);

        //return mapped.ToList();

        return new OkServiceResult<IAsyncEnumerable<TDtoFull>>(result);
    }

    public virtual async Task<ServiceResultBase<TEntity?>> GetEntityByIdAsync(
        TKey id,
        IUserProfile userProfile,
        Func<IQueryable<TEntity>, IUserProfile, Task<IQueryable<TEntity>>>? customAction = null,
        CancellationToken cancellationToken = default)
    {
        if (id is int intId)
            InvalidIdArgumentException.ThrowIfZero(intId);

        var query = QueryableOfActive;

        if (customAction != null)
            query = await customAction(query, userProfile);

        var result = await query.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);

        return result;
    }

    public virtual async Task<ServiceResultBase<TDtoFull?>> GetByIdAsync(
        TKey id,
        IUserProfile userProfile,
        Func<IQueryable<TEntity>, IUserProfile, Task<IQueryable<TEntity>>>? customAction = null,
        CancellationToken cancellationToken = default)
    {

        InvalidIdArgumentException.ThrowIfInvalid(id);

        var query = QueryableOfActive;

        if (customAction != null)
            query = await customAction(query, userProfile);

        var entity = await query.FirstOrDefaultAsync(x => x.Id.Equals(id), cancellationToken);

        if (entity is null)
            return null;

        var result = Mapper.Map<TDtoFull>(entity);

        return result;
    }

    public virtual async Task<ServiceResultBase<TEntity>> InsertAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        CheckInsertValidity(entity.Id);

        entity = await HandleInsertAsync(entity, cancellationToken);

        return entity;
    }

    public virtual async Task<ServiceResultBase<TDtoFull>> InsertAsync(TDtoFull entity, CancellationToken cancellationToken = default)
    {
        var mapped = Mapper.Map<TEntity>(entity);

        CheckInsertValidity(mapped.Id);

        mapped = await HandleInsertAsync(mapped, cancellationToken);

        entity = Mapper.Map<TDtoFull>(mapped);

        return entity;
    }

    public virtual async Task<ServiceResultBase<TEntity>> UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
    {
        await CheckUpdateValidityAsync(entity.Id, cancellationToken);

        var result = await HandleUpdateAsync(entity, cancellationToken);

        return result.Entity;
    }

    public virtual async Task<ServiceResultBase<TDtoFull>> UpdateAsync(TDtoFull entity, CancellationToken cancellationToken = default)
    {
        var mapped = Mapper.Map<TEntity>(entity);

        await CheckUpdateValidityAsync(mapped.Id, cancellationToken);

        var result = await HandleUpdateAsync(mapped, cancellationToken);

        return Mapper.Map<TDtoFull>(result.Entity);
    }

    public async Task<ServiceResultBase<int>> PatchUpdateAsync(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
        ArgumentNullException.ThrowIfNull(setPropertyCalls, nameof(setPropertyCalls));

        var resultedCount = await Set
            .Where(predicate)
            .ExecuteUpdateAsync(setPropertyCalls, cancellationToken);

        return new OkServiceResult<int>(resultedCount);
    }

    public async Task<ServiceResultBase<int>> PatchUpdateAsync(
        TKey id,
        Expression<Func<SetPropertyCalls<TEntity>, SetPropertyCalls<TEntity>>> setPropertyCalls,
        CancellationToken cancellationToken = default)
    {
        if (id is int intId)
            InvalidIdArgumentException.ThrowIfZero(intId);

        ArgumentNullException.ThrowIfNull(setPropertyCalls, nameof(setPropertyCalls));

        var resultedCount = await Set
            .Where(x => x.Id.Equals(id))
            .ExecuteUpdateAsync(setPropertyCalls, cancellationToken);

        return new OkServiceResult<int>(resultedCount);
    }

    public async Task<ServiceResultBase<int>> PatchUpdateAsync<TResult>(
        Expression<Func<TEntity, bool>> predicate,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<SetPropertyCalls<TResult>, SetPropertyCalls<TResult>>> setPropertyCalls,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(predicate, nameof(predicate));
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));
        ArgumentNullException.ThrowIfNull(setPropertyCalls, nameof(setPropertyCalls));

        var resultedCount = await Set
            .Where(predicate)
            .Select(selector)
            .ExecuteUpdateAsync(setPropertyCalls, cancellationToken);

        return new OkServiceResult<int>(resultedCount);
    }

    public async Task<ServiceResultBase<int>> PatchUpdateAsync<TResult>(
        TKey id,
        Expression<Func<TEntity, TResult>> selector,
        Expression<Func<SetPropertyCalls<TResult>, SetPropertyCalls<TResult>>> setPropertyCalls,
        CancellationToken cancellationToken = default)
    {
        if (id is int intId)
            InvalidIdArgumentException.ThrowIfZero(intId);
        ArgumentNullException.ThrowIfNull(selector, nameof(selector));
        ArgumentNullException.ThrowIfNull(setPropertyCalls, nameof(setPropertyCalls));

        var resultedCount = await Set
            .Where(x => x.Id.Equals(id))
            .Select(selector)
            .ExecuteUpdateAsync(setPropertyCalls, cancellationToken);
        return new OkServiceResult<int>(resultedCount);
    }

    public virtual async Task<ServiceResultBase<bool>> DeactivateByIdAsync(
        TKey id,
        IUserProfile userProfile,
        Func<IQueryable<TEntity>, IUserProfile, Task<IQueryable<TEntity>>>? customAction = null,
        CancellationToken cancellationToken = default)
    {

        await CheckUpdateValidityAsync(id, cancellationToken);

        var entity = await Set.FindAsync([id], cancellationToken);

        entity!.Active = false;

        var saved = await HandleSaveChangesAsync(cancellationToken);

        return saved;
    }
}