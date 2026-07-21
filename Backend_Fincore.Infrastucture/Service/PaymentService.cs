using AutoMapper;
using Backend_Fincore.Application.DTOs;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backend_Fincore.Infrastucture.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext db;

        IMapper mapper;
        public PaymentService(AppDbContext db,IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task AddPayment(PaymentCUDTO paymentDTO)
        {
            bool isValid = false;

            if (paymentDTO.MasterType == "APInvoice")
            {
                isValid = await db.APInvoice.AnyAsync(x => x.APInvoiceId == paymentDTO.MasterId);
            }
            else if (paymentDTO.MasterType == "ARInvoice")
            {
                isValid = await db.ARInvoice.AnyAsync(x => x.ARInvoiceId == paymentDTO.MasterId);
            }
            else if (paymentDTO.MasterType == "ExpenseClaim")
            {
                isValid = await db.ExpenseClaim.AnyAsync(x => x.ExpenseClaimId == paymentDTO.MasterId);
            }
            else
            {
                throw new Exception("Invalid Master Type.");
            }

            if (!isValid)
            {
                throw new Exception($"{paymentDTO.MasterType} not found.");
            }

            var data = mapper.Map<Payment>(paymentDTO);

            //data.CreatedBy=userid
            data.CreatedBy = 1;

            await db.Payment.AddAsync(data);
            await db.SaveChangesAsync();
        }

        public async Task<bool> DeletePaymentById(int id)
        {
            var payment = await db.Payment.FirstOrDefaultAsync(x => x.PaymentId == id);

            if(payment != null)
            {
                db.Payment.Remove(payment);
                db.SaveChangesAsync();

                return true;
            }

            return false;
        }

        public async Task<List<PaymentDTO>> GetAllPayment()
        {
            var res = await db.Payment.ToListAsync();

            var data = mapper.Map<List<PaymentDTO>>(res);

            return data;
        }

        public async Task<PaymentDTO> GetPaymentById(int id)
        {
            var res = await db.Payment.FirstOrDefaultAsync(x => x.PaymentId == id);

            var data = mapper.Map<PaymentDTO>(res);

            return data;
        }


        public async Task UpdatePayment(PaymentCUDTO paymentDto, int id)
        {
            var payment = await db.Payment.FirstOrDefaultAsync(x => x.PaymentId == id);


            if (payment == null)
            {
                throw new Exception("Payment not found.");
            }

            bool isValid = false;

            if (paymentDto.MasterType == "APInvoice")
            {
                isValid = await db.APInvoice.AnyAsync(x => x.APInvoiceId == paymentDto.MasterId);
            }
            else if (paymentDto.MasterType == "ARInvoice")
            {
                isValid = await db.ARInvoice.AnyAsync(x => x.ARInvoiceId == paymentDto.MasterId);
            }
            else if (paymentDto.MasterType == "ExpenseClaim")
            {
                isValid = await db.ExpenseClaim.AnyAsync(x => x.ExpenseClaimId == paymentDto.MasterId);
            }
            else
            {
                throw new Exception("Invalid Master Type.");
            }

            if (!isValid)
            {
                throw new Exception($"{paymentDto.MasterType} not found.");
            }

            payment.MasterId = paymentDto.MasterId;
            payment.MasterType = paymentDto.MasterType;
            payment.Amount = paymentDto.Amount;
            payment.PaymentDate = paymentDto.PaymentDate;
            payment.TransactionType = paymentDto.TransactionType;
            payment.PaymentMode = paymentDto.PaymentMode;
            payment.Remarks = paymentDto.Remarks;

            payment.ModifiedBy = 1;
            payment.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();
        }
    }
}
