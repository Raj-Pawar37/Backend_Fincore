using Backend_Fincore.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Application.Interface
{
    public interface IPaymentService
    {

        Task<List<PaymentDTO>> GetAllPayment();

        Task<PaymentDTO> GetPaymentById(int id);

        Task AddPayment(PaymentCUDTO payment);

        Task UpdatePayment(PaymentCUDTO payment, int id);

        Task<bool> DeletePaymentById(int id);
    }
}
