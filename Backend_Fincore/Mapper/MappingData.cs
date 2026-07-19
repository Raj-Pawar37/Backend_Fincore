using AutoMapper;
using Backend_Fincore.DTOs.GRN;
using Backend_Fincore.DTOs.PurchaseOrder;
using Backend_Fincore.DTOs.PurchaseOrderItem;
using Backend_Fincore.Models;

namespace Backend_Fincore.Mapper;

public class MappingData : Profile
{

    public MappingData()
    {
        CreateMap<PurchaseOrder, PurchaseOrderDTO>();
        CreateMap<PurchaseOrderCUDTO, PurchaseOrder>();

        CreateMap<PurchaseOrderItem, PurchaseOrderItemDTO>();
        CreateMap<PurchaseOrderItemCUDTO, PurchaseOrderItem>();

        CreateMap<GRN, GRNDTO>().ForMember(x => x.PONumber, x => x.MapFrom(x => x.PurchaseOrder.PONumber))
            .ForMember(x => x.Username, x => x.MapFrom(x => x.ReceivedByUser.Username));


        CreateMap<GRNCUDTO, GRN>();


    }

   
}
