using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.ExpenseClaim;
using Backend_Fincore.Application.DTOs.OpexRequest;
using Backend_Fincore.Application.DTOs.PurchaseRequisition;
using Backend_Fincore.Application.DTOs.RFQ;
using Backend_Fincore.Application.DTOs.WorkOrder;
using Backend_Fincore.DTOs;
using Backend_Fincore.DTOs.APInvoice;
using Backend_Fincore.DTOs.GRN;
using Backend_Fincore.DTOs.PurchaseOrder;
using Backend_Fincore.DTOs.PurchaseOrderItem;
using Backend_Fincore.Models;



namespace Backend_Fincore.Mapper;

public class MappingData : Profile
{

    public MappingData()
    {
        //PurchaseOrder
        CreateMap<PurchaseOrder, PurchaseOrderDTO>();
        CreateMap<PurchaseOrderCUDTO, PurchaseOrder>();

        //PurchaseOrderItem
        CreateMap<PurchaseOrderItem, PurchaseOrderItemDTO>();
        CreateMap<PurchaseOrderItemCUDTO, PurchaseOrderItem>();

        //GRN
        CreateMap<GRN, GRNDTO>().ForMember(x => x.PONumber, x => x.MapFrom(x => x.PurchaseOrder.PONumber))
                                .ForMember(x => x.Username, x => x.MapFrom(x => x.ReceivedByUser.Username));


        CreateMap<GRNCUDTO, GRN>();

        //APInvoice
        CreateMap<APInvoice, APInvoiceDTO>().ForMember(x => x.VendorName,
                                            x => x.MapFrom(x => x.Vendor.VendorName));
        CreateMap<APInvoiceCUDTO, APInvoice>();

        //Assets

        CreateMap<Asset, AssetsDTO>().ForMember(x => x.GRNNumber, x => x.MapFrom(x => x.GRN.GRNNumber))
                                     .ForMember(x => x.ItemName, x => x.MapFrom(x => x.PurchaseOrderItem.ItemName))
                                     .ForMember(x => x.AssignedUserName, x => x.MapFrom(x => x.AssignedToUser.Username));

        CreateMap<AssetsCUDTO, Asset>();

        //Payment

        CreateMap<Payment, PaymentDTO>();

        CreateMap<PaymentCUDTO, Payment>();




        //CreateMap<QuotationItem, PurchaseOrderItem>();

        //employee
        CreateMap<Employee, EmployeeReadDTO>()
            .ForMember(d => d.CompanyName, x => x.MapFrom(y => y.Company.CompanyName))
            .ForMember(d => d.DepartmentName, x => x.MapFrom(y => y.Department.DepartmentName))
            .ForMember(d => d.ReportingManagerName, x => x.MapFrom(y => y.ReportingManager != null
            ? y.ReportingManager.FirstName + " " + y.ReportingManager.LastName
            : null));

        CreateMap<Employee, EmployeeWriteDTO>()
            .ReverseMap();

        //user
        CreateMap<User, UserReadDTO>()
              .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Role.RoleName));
        CreateMap<User, UserWriteDTO>().ReverseMap();


        //company
        CreateMap<Company, CompanyReadDTO>()
            .ForMember(d => d.CountryName, x => x.MapFrom(y => y.Country.CountryName))
            .ForMember(d => d.StateName, x => x.MapFrom(y => y.State.StateName))
            .ForMember(d => d.CityName, x => x.MapFrom(y => y.City.CityName));
        CreateMap<CompanyWriteDTO, Company>().ReverseMap();


        //Vendor

        CreateMap<Vendor, VendorReadDTO>()
            .ForMember(d => d.CompanyName,
                x => x.MapFrom(y => y.Company.CompanyName));

        CreateMap<Vendor, VendorWriteDTO>()
            .ReverseMap();

        // < src , dest >
        CreateMap<Role, RoleDTO>().ReverseMap();
        //CreateMap<RoleDTO, Role>();
        //CreateMap<Role, RoleDTO>();

        CreateMap<Permission, PermissionDTO>().ReverseMap();


        // OpexRequest 
        // Opex Request
        CreateMap<OpexRequest, OpexRequestReadDTO>();

        CreateMap<OpexRequestWriteDTO, OpexRequest>().ReverseMap();


        // Expense Claim
        CreateMap<ExpenseClaim, ExpenseClaimReadDTO>();

        CreateMap<ExpenseClaim, ExpenseClaimWriteDTO>()
            .ReverseMap();

        // Work Order
        CreateMap<WorkOrder, WorkOrderReadDTO>();

        CreateMap<WorkOrder, WorkOrderWriteDTO>()
            .ReverseMap();


        CreateMap<BudgetCategory, BudgetCategoryReadDTO>();//Ritik

        CreateMap<BudgetCategoryWriteDTO, BudgetCategory>().ReverseMap();//Ritik

        CreateMap<BudgetWriteDTO, Budget>();//Ritik

        CreateMap<Budget, BudgetReadDTO>()
            .ForMember(dest => dest.CompanyName,
                opt => opt.MapFrom(src => src.Company.CompanyName))
            .ForMember(dest => dest.DepartmentName,
                opt => opt.MapFrom(src => src.Department.DepartmentName))
            .ForMember(dest => dest.ApprovedByName,
                opt => opt.MapFrom(src => src.ApprovedByUser != null ? src.ApprovedByUser.Username : null));//Ritik

        CreateMap<BudgetLineWriteDTO, BudgetLine>();

        CreateMap<BudgetLine, BudgetLineReadDTO>()
            .ForMember(dest => dest.FinancialYear,
                opt => opt.MapFrom(src => src.Budget.FinancialYear))
            .ForMember(dest => dest.CompanyName,
                opt => opt.MapFrom(src => src.Budget.Company.CompanyName))
            .ForMember(dest => dest.DepartmentName,
                opt => opt.MapFrom(src => src.Budget.Department.DepartmentName))
            .ForMember(dest => dest.BudgetCategoryName,
                opt => opt.MapFrom(src => src.BudgetCategory.CategoryName));



        CreateMap<PurchaseRequisitionCreateDto, PurchaseRequisition>();
        CreateMap<PurchaseRequisitionUpdateDto, PurchaseRequisition>();
        CreateMap<PurchaseRequisition, PurchaseRequisitionResponseDto>();

        CreateMap<RFQCreateDto, RFQ>();
        CreateMap<RFQItemCreateDto, RFQItem>();
        CreateMap<RFQ, RFQResponseDto>();


    }


}
