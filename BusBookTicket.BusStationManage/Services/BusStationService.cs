using AutoMapper;
using BusBookTicket.AddressManagement.Services.WardService;
using BusBookTicket.AddressManagement.Utilities;
using BusBookTicket.BusStationManage.DTOs.Requests;
using BusBookTicket.BusStationManage.DTOs.Responses;
using BusBookTicket.BusStationManage.Paging;
using BusBookTicket.BusStationManage.Specification;
using BusBookTicket.BusStationManage.Utils;
using BusBookTicket.Core.Common;
using BusBookTicket.Core.Infrastructure.Interfaces;
using BusBookTicket.Core.Models.Entity;
using BusBookTicket.Core.Utils;

namespace BusBookTicket.BusStationManage.Services;

public class BusStationService : IBusStationService
{
    #region -- Properties --

    private readonly IMapper _mapper;
    private readonly IGenericRepository<BusStation> _repository;
    private readonly IWardService _wardService;
    #endregion -- Properties --

    #region -- Public Method --
    public BusStationService(IMapper mapper, IUnitOfWork unitOfWork, IWardService wardService)
    {
        _repository = unitOfWork.GenericRepository<BusStation>();
        this._mapper = mapper;
        _wardService = wardService;
    }
    public async Task<BusStationResponse> GetById(int id)
    {
        BusStationSpecification busStationSpecification = new BusStationSpecification(id);
        BusStation busStation = await _repository.Get(busStationSpecification);
        BusStationResponse response = _mapper.Map<BusStationResponse>(busStation);
        response.AddressDb = await GetFullAddress(response.Address, response.WardId);
        return response;
    }

    public async Task<List<BusStationResponse>> GetAll()
    {
        BusStationSpecification busStationSpecification = new BusStationSpecification();
        List<BusStationResponse> responses = new List<BusStationResponse>();
        List<BusStation> busStations = await _repository.ToList(busStationSpecification);
        responses = await AppUtils.MappObject<BusStation, BusStationResponse>(busStations, _mapper);
        for (int i = 0; i < responses.Count; i++)
        {
            responses[i].AddressDb = await GetFullAddress(responses[i].Address, responses[i].WardId);

        }
        return responses;
    }
    
    public async Task<List<BusStationResponse>> GetAllByAdmin()
    {
        BusStationSpecification busStationSpecification = new BusStationSpecification(false);
        List<BusStationResponse> responses = new List<BusStationResponse>();
        List<BusStation> busStations = await _repository.ToList(busStationSpecification);
        responses = await AppUtils.MappObject<BusStation, BusStationResponse>(busStations, _mapper);
        for (int i = 0; i < responses.Count; i++)
        {
            responses[i].AddressDb = await GetFullAddress(responses[i].Address, responses[i].WardId);
        }
        return responses;
    }

    public async Task<StationPagingResult> GetAll(StationPaging request)
    {
        BusStationSpecification busStationSpecification = new BusStationSpecification(paging: request);
        List<BusStationResponse> stationResponses = new List<BusStationResponse>();
        StationPagingResult response = new StationPagingResult();
        response.PageIndex = request.PageIndex;
        response.PageSize = request.PageSize;
        List<BusStation> busStations = await _repository.ToList(busStationSpecification);
        response.Items = await AppUtils.MappObject<BusStation, BusStationResponse>(busStations, _mapper);
        for (int i = 0; i < response.Items.Count; i++)
        {
            response.Items[i].AddressDb = await GetFullAddress(response.Items[i].Address, response.Items[i].WardId);
        }
        return response;
    }

    public async Task<bool> Update(BST_FormUpdate entity, int id, int userId)
    {
        BusStation busStation = _mapper.Map<BusStation>(entity);
        busStation.Id = id;
        await _repository.Update(busStation, userId);
        return true;
    }

    public async Task<bool> Delete(int id, int userId)
    {
        BusStationSpecification busStationSpecification = new BusStationSpecification(id);
        BusStation busStation = await _repository.Get(busStationSpecification);
        busStation.Status = (int)EnumsApp.Delete;
        await _repository.Update(busStation, userId);
        return true;
    }

    public async Task<bool> Create(BST_FormCreate entity, int userId)
    {
        BusStationSpecification specification = new BusStationSpecification(entity.Name, false);
        if (await _repository.CheckIsExist(specification))
            throw new ExceptionDetail(BusStationConstants.EXIST_RESOURCE);
        
        BusStation busStation = _mapper.Map<BusStation>(entity);
        await _repository.Create(busStation, userId);
        return true;
    }

    public async Task<bool> ChangeIsActive(int id, int userId)
    {
        BusStationSpecification busStationSpecification = new BusStationSpecification(id, false);
        BusStation busStation = await _repository.Get(busStationSpecification);
        return await _repository.ChangeStatus(busStation, userId, (int)EnumsApp.Active);
    }

    public async Task<bool> ChangeIsLock(int id, int userId)
    {
        BusStationSpecification busStationSpecification = new BusStationSpecification(id, false);
        BusStation busStation = await _repository.Get(busStationSpecification);
        return await _repository.ChangeStatus(busStation, userId, (int)EnumsApp.Lock);
    }

    public async Task<bool> ChangeToWaiting(int id, int userId)
    {
        BusStationSpecification busStationSpecification = new BusStationSpecification(id, false);
        BusStation busStation = await _repository.Get(busStationSpecification);
        return await _repository.ChangeStatus(busStation, userId, (int)EnumsApp.Waiting);
    }

    public async Task<bool> ChangeToDisable(int id, int userId)
    {
        BusStationSpecification busStationSpecification = new BusStationSpecification(id, false);
        BusStation busStation = await _repository.Get(busStationSpecification);
        return await _repository.ChangeStatus(busStation, userId, (int)EnumsApp.Disable);
    }

    public Task<bool> CheckToExistById(int id)
    {
        throw new NotImplementedException();
    }

    public Task<bool> CheckToExistByParam(string param)
    {
        throw new NotImplementedException();
    }

    public async Task<BusStationResponse> GetStationByName(string name)
    {
        BusStationSpecification busStationSpecification = new BusStationSpecification(name);
        BusStation busStation = await _repository.Get(busStationSpecification);
        BusStationResponse response =  _mapper.Map<BusStationResponse>(busStation);
        response.AddressDb = await GetFullAddress(response.Address, response.WardId);
        return response;
    }

    public async Task<List<BusStationResponse>> GetStationByLocation(string location)
    {
        BusStationSpecification busStationSpecification = new BusStationSpecification("", location);
        List<BusStation> busStations = await _repository.ToList(busStationSpecification);
        List<BusStationResponse> responses = await AppUtils.MappObject<BusStation, BusStationResponse>(busStations, _mapper);
        for (int i = 0; i < responses.Count; i++)
        {
            responses[i].AddressDb = await GetFullAddress(responses[i].Address, responses[i].WardId);
        }
        return responses;
    }

    public async Task<List<BusStationResponse>> GetAllStationInBus(int busId)
    {
        BusStationSpecification specification = new BusStationSpecification(0, busId);
        List<BusStation> busStations = await _repository.ToList(specification);
        List<BusStationResponse> responses = await AppUtils.MappObject<BusStation, BusStationResponse>(busStations, _mapper);
        for (int i = 0; i < responses.Count; i++)
        {
            responses[i].AddressDb = await GetFullAddress(responses[i].Address, responses[i].WardId);
        }
        return responses;
    }

    #endregion

    #region -- Private Method --

    private async Task<string> GetFullAddress(string address, int wardId)
    {
        string addressDb = address + ", " + await AddressResponse.GetAddressDb(wardId, _wardService);
        return addressDb;
    }
    #endregion -- Private Method --

}