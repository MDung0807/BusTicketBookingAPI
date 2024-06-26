﻿using AutoMapper;
using BusBookTicket.Application.Notification.Modal;
using BusBookTicket.Application.Notification.Services;
using BusBookTicket.Core.Common.Exceptions;
using BusBookTicket.Core.Infrastructure.Interfaces;
using BusBookTicket.Core.Models.Entity;
using BusBookTicket.Core.Utils;
using BusBookTicket.PriceManage.DTOs.Requests;
using BusBookTicket.PriceManage.DTOs.Responses;
using BusBookTicket.PriceManage.Paging;
using BusBookTicket.PriceManage.Specification;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BusBookTicket.PriceManage.Services;

public class PriceService : IPriceService
{
    private readonly IMapper _mapper;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<Prices> _repository;
    private readonly INotificationService _notification;


    public PriceService(IMapper mapper, IUnitOfWork unitOfWork, INotificationService notification)
    {
        _unitOfWork = unitOfWork;
        _notification = notification;
        _mapper = mapper;
        _repository = _unitOfWork.GenericRepository<Prices>();
    }
    
    public Task<PriceResponse> GetById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<List<PriceResponse>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<bool> Update(PriceUpdate entity, int id, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(int id, int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> Create(PriceCreate entity, int userId)
    {
        Prices price = _mapper.Map<Prices>(entity);
        price.Company = new Company();
        price.Company.Id = userId;
        price.Status = (int)EnumsApp.Waiting;
        await _repository.Create(price, userId: userId);
        await SendNotification(price, entity.CompanyName);
        return true;
    }

    public async Task<bool> ChangeIsActive(int id, int userId)
    {
        PriceSpecification specification = new PriceSpecification(id: id, checkStatus: false, getIsChange: true);
        Prices price = await _repository.Get(specification, checkStatus: false) ?? throw new ExceptionDetail(AppConstants.NOT_FOUND);
        await _repository.ChangeStatus(price, userId: userId, (int)EnumsApp.Active);
        return true;
    }

    public Task<bool> ChangeIsLock(int id, int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ChangeToWaiting(int id, int userId)
    {
        PriceSpecification specification = new PriceSpecification(id: id, checkStatus: false, getIsChange: true);
        Prices price = await _repository.Get(specification, checkStatus: false);
        await _repository.ChangeStatus(price, userId: userId, (int)EnumsApp.Waiting);
        return true;
    }

    public Task<bool> ChangeToDisable(int id, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckToExistById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckToExistByParam(string param)
    {
        throw new NotImplementedException();
    }


    public async Task<PricePagingResult> GetAllByAdmin(PricePaging pagingRequest)
    {
        PriceSpecification specification =
            new PriceSpecification(checkStatus: false, paging: pagingRequest);
        List<Prices> prices = await _repository.ToList(specification);
        int count = await _repository.Count(new PriceSpecification(checkStatus:false));

        var result = AppUtils.ResultPaging<PricePagingResult, PriceResponse>(
            pagingRequest.PageIndex,
            pagingRequest.PageSize,
            count: count,
            items: await AppUtils.MapObject<Prices, PriceResponse>(prices,
                _mapper));
        return result;
    }

    public Task<PricePagingResult> GetAll(PricePaging pagingRequest)
    {
        throw new NotImplementedException();
    }

    public async Task<PricePagingResult> GetAll(PricePaging pagingRequest, int idMaster)
    {
        PriceSpecification specification = new PriceSpecification(companyId:idMaster, paging: pagingRequest, checkStatus:false);
        int count = await _repository.Count(new PriceSpecification(companyId: idMaster, checkStatus:false));
        List<Prices> pricesList = await _repository.ToList(specification);

        var result = AppUtils.ResultPaging<PricePagingResult, PriceResponse>(
            pagingRequest.PageIndex,
            pagingRequest.PageSize,
            count: count,
            items: await AppUtils.MapObject<Prices, PriceResponse>(pricesList, _mapper));
        return result;
    }

    public Task<bool> DeleteHard(int id)
    {
        throw new NotImplementedException();
    }

    public async Task<PriceResponse> GetInRoute(int routeId, int companyId)
    {
        PriceSpecification specification = new PriceSpecification(routeId: routeId, companyId: companyId);
        Prices price = await _repository.Get(specification);
        return _mapper.Map<PriceResponse>(price);
    }

    #region --Private Method --

    private async Task SendNotification( Prices prices, string companyName)
    {
        AddNewNotification newNotification = new AddNewNotification
        {
            Content = $"{companyName} Đã tạo bảng giá",
            Actor = "ADMIN_1",
            Href = AppConstants.PRICETYPE,
            Sender = $"{companyName}"
        };
        await _notification.InsertNotification(newNotification, prices.Company.Id);
    }

    #endregion --Private Method --
}