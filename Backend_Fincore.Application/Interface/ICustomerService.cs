using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.DTOs.Customer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public  interface ICustomerService
    {

        Task<List<CustomerReadDTO>> GetAll(PaginationDTO pagination);
        Task<int> GetTotalCustomerRecords(string? search);

        Task<CustomerReadDTO> GetById(int id);


        Task<CustomerReadDTO> AddCutomer(CustomerWriteDTO c);


        Task<bool> DeleteCustomer(int id);


        Task<bool> UpdateCustomer(int id, CustomerWriteDTO c);
    }
}
