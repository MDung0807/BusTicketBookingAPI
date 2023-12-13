﻿using AutoMapper;
using BusBookTicket.Application.MailKet.DTO.Request;
using BusBookTicket.Application.MailKet.Service;
using BusBookTicket.BillManage.DTOs.Requests;
using BusBookTicket.BillManage.DTOs.Responses;
using BusBookTicket.BillManage.Paging;
using BusBookTicket.BillManage.Services.BillItems;
using BusBookTicket.BillManage.Specification;
using BusBookTicket.BillManage.Utilities;
using BusBookTicket.Core.Application.Paging;
using BusBookTicket.Core.Infrastructure;
using BusBookTicket.Core.Infrastructure.Interfaces;
using BusBookTicket.Core.Models.Entity;
using BusBookTicket.Core.Utils;
using BusBookTicket.CustomerManage.Specification;
using BusBookTicket.Ticket.DTOs.Response;
using BusBookTicket.Ticket.Services.TicketItemServices;
using BusBookTicket.Ticket.Specification;

namespace BusBookTicket.BillManage.Services.Bills;

public class BillService : IBillService
{
    private readonly IMapper _mapper;
    private readonly IBillItemService _billItemService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IGenericRepository<Bill> _repository;
    private readonly ITicketItemService _ticketItemService;
    private readonly IMailService _mailService;
    private readonly IGenericRepository<Customer> _customerRepo;
    private readonly IGenericRepository<Ticket_BusStop> _ticketBusStop;

    public BillService(
        IMapper mapper,
        ITicketItemService itemService,
        IBillItemService billItemService,
        IUnitOfWork unitOfWork,
        IMailService mailService
        )
    {
        _billItemService = billItemService;
        _unitOfWork = unitOfWork;
        _repository = unitOfWork.GenericRepository<Bill>();
        _mapper = mapper;
        _ticketItemService = itemService;
        _mailService = mailService;
        _customerRepo = unitOfWork.GenericRepository<Customer>();
        _ticketBusStop = unitOfWork.GenericRepository<Ticket_BusStop>();

    }
    public async Task<BillResponse> GetById(int id)
    {
        BillSpecification specification = new BillSpecification(id: id, checkStatus: false);
        Bill bill = await _repository.Get(specification);
        BillResponse response = _mapper.Map<BillResponse>(bill);
        response.Items = await _billItemService.GetItemInBill(bill.Id);
        return response;
    }

    public Task<List<BillResponse>> GetAll()
    {
        throw new NotImplementedException();
    }

    public Task<bool> Update(BillRequest entity, int id, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Delete(int id, int userId)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> Create(BillRequest entity, int userId)
    {
        List<BillItemRequest> itemsRequest = entity.ItemsRequest;
        await _unitOfWork.BeginTransaction();
        try
        {
            // Create bill
            Bill bill = _mapper.Map<Bill>(entity);
            CustomerSpecification customerSpecification = new CustomerSpecification(userId);

            Ticket_BusStop ticketBusStopEnd = await _ticketBusStop.Get(new TicketBusStopSpecification(entity.BusStationEndId, "Get"));
            Ticket_BusStop ticketBusStopStart= await _ticketBusStop.Get(new TicketBusStopSpecification(entity.BusStationStartId, "Get"));

            bill.Customer = new Customer();
            bill.Customer.Id = userId;
            bill = await _repository.Create(bill, userId);
            

            // Create item
            foreach (BillItemRequest item in itemsRequest)
            {
                item.BillId = bill.Id;
                BillItem billItem = await _billItemService.CreateBillItem(item, userId);
                //Cacl total Price
                TicketItemResponse ticketItem = await _ticketItemService.GetById(item.TicketItemId);
                
                // Change status in ticket item
                await _ticketItemService.ChangeStatusToWaitingPayment(item.TicketItemId, userId);

                bill.TotalPrice += ticketItem.Price + ticketBusStopStart.DiscountPrice + ticketBusStopEnd.DiscountPrice;
                
            }

            // Update total price in bill
            bill.Status = (int)EnumsApp.AwaitingPayment;
            await _repository.Update(bill, userId);
            
            // Change status in Bill
            // await ChangeStatusToWaitingPayment(bill.Id, userId);
            Customer customer = await _customerRepo.Get(customerSpecification);
            //Send mail
            MailRequest mailRequest = new MailRequest();
            mailRequest.ToMail = customer.Email;
            mailRequest.Message = $"Bạn vừa có một hóa đơn cho chuyến đi từ: {ticketBusStopStart.BusStop.BusStation.Name} đến {ticketBusStopEnd.BusStop.BusStation.Name}";
            mailRequest.Content = $"Giá: {bill.TotalPrice}";
            mailRequest.Subject = "Hóa đơn của bạn";
            mailRequest.FullName = customer.FullName;
            await _mailService.SendEmailAsync(mailRequest);

            await _unitOfWork.SaveChangesAsync();
            
            _unitOfWork.Dispose();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            _unitOfWork.Dispose();
            throw new Exception(BillConstants.ERROR);
        }

        return true;
    }

    public Task<bool> ChangeIsActive(int id, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ChangeIsLock(int id, int userId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ChangeToWaiting(int id, int userId)
    {
        throw new NotImplementedException();
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

    public Task<BillPagingResult> GetAllByAdmin(BillPaging pagingRequest)
    {
        throw new NotImplementedException();
    }

    public Task<BillPagingResult> GetAll(BillPaging pagingRequest)
    {
        throw new NotImplementedException();
    }

    public Task<BillPagingResult> GetAll(BillPaging pagingRequest, int idMaster)
    {
        throw new NotImplementedException();
    }

    public Task<bool> DeleteHard(int id)
    {
        throw new NotImplementedException();
    }

    public Task<PagingResult<BillResponse>> GetAllByAdmin(PagingRequest pagingRequest)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> ChangeStatusToWaitingPayment(int id, int userId)
    {
        BillSpecification specification = new BillSpecification(id, checkStatus:false);
        Bill bill = await _repository.Get(specification);
        bill.Customer = null;
        bill.BusStationEnd = null;
        bill.BusStationStart = null;

        return await _repository.ChangeStatus(bill, userId, (int)EnumsApp.AwaitingPayment);
    }

    public async Task<bool> ChangeStatusToPaymentComplete(int id, int userId)
    {
        BillSpecification specification = new BillSpecification(id, checkStatus:false);
        Bill bill = await _repository.Get(specification);

        return await _repository.ChangeStatus(bill, userId, (int)EnumsApp.PaymentComplete);
    }

    public async Task<BillPagingResult> GetAllBillInUser(BillPaging paging, int userId)
    {
        BillSpecification billSpecification = new BillSpecification(userId:userId, checkStatus:false, paging: paging);
        List<Bill> bills = await _repository.ToList(billSpecification);
        BillPagingResult result = new BillPagingResult();
        int count = await _repository.Count(new BillSpecification(userId:userId, checkStatus:false));

        List<BillResponse> billResponses = await AppUtils.MapObject<Bill, BillResponse>(bills, _mapper);

        foreach (var item in billResponses)
        {
            item.Items = await _billItemService.GetItemInBill(item.Id);
        }
        result = AppUtils.ResultPaging<BillPagingResult, BillResponse>(
            paging.PageIndex,
            paging.PageSize,
            count,
            items: billResponses
            );
        return result;
    }

    public async Task<bool> ChangeCompleteStatus(int billId, int userId)
    {
        BillSpecification specification = new BillSpecification(billId, checkStatus:false);
        Bill bill = await _repository.Get(specification);
        await _repository.ChangeStatus(bill, userId: userId, (int)EnumsApp.Active);
        return true;
    }

    public async Task<BillResponse> GetBillByUserAndBus(int userId, int busId)
    {
        BillSpecification specification = new BillSpecification(userId: userId, busId: busId);
        Bill bill =await _repository.Get(specification);
        BillResponse response = _mapper.Map<BillResponse>(bill);
        return response;
    }
}