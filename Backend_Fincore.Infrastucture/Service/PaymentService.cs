using AutoMapper;
using Backend_Fincore.Application.DTOs.Payment;
using Backend_Fincore.Application.Interface;
using Backend_Fincore.Data;
using Backend_Fincore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Buffers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Backend_Fincore.Infrastucture.Service
{
    public class PaymentService : IPaymentService
    {
        private readonly AppDbContext db;

        IMapper mapper;
        public PaymentService(AppDbContext db, IMapper mapper)
        {
            this.db = db;
            this.mapper = mapper;
        }

        public async Task AddPayment(PaymentCUDTO paymentDTO)
        {

            if (paymentDTO.Amount <= 0)
            {
                throw new ArgumentException("Payment must be Greater than Zero");
            }

            decimal totalPayableAmount;
            switch (paymentDTO.MasterType)
            {

                case "APInvoice":
                    {
                        var invoice = await db.APInvoice.FirstOrDefaultAsync(x => x.APInvoiceId == paymentDTO.MasterId);

                        if (invoice == null)
                        {
                            throw new Exception("AP Invoice not found.");
                        }

                        totalPayableAmount = invoice.InvoiceAmount;
                        break;
                    }

                case "ARInvoice":
                    {
                        var invoice = await db.ARInvoice.FirstOrDefaultAsync(x => x.ARInvoiceId == paymentDTO.MasterId);

                        if (invoice == null)
                        {
                            throw new Exception("AR Invoice not found.");
                        }

                        totalPayableAmount = invoice.InvoiceAmount;
                        break;
                    }

                case "ExpenseClaim":
                    {
                        var expenseClaim = await db.ExpenseClaim.FirstOrDefaultAsync(x => x.ExpenseClaimId == paymentDTO.MasterId);

                        if (expenseClaim == null)
                        {
                            throw new Exception("Expense Claim not found.");
                        }

                        totalPayableAmount = expenseClaim.ExpenseAmount;
                        break;
                    }

                default:
                    throw new ArgumentException("MasterType is Invalid");


            }

            decimal alreadyPaid = await db.Payment.Where(x => x.MasterId == paymentDTO.MasterId && x.MasterType == paymentDTO.MasterType).SumAsync(x => (decimal?)x.Amount) ?? 0;

            decimal remaingAmount = totalPayableAmount - alreadyPaid;

            if (remaingAmount <= 0)
            {
                throw new Exception("Payment has been completed Already");
            }

            if (remaingAmount < paymentDTO.Amount)
            {
                throw new Exception("Payment Can be exceeed than Remaning Invoice Amount");
            }

            var data = mapper.Map<Payment>(paymentDTO);

            //data.CreatedBy=userid
            data.CreatedBy = paymentDTO.CreatedBy;

            await db.Payment.AddAsync(data);
            await db.SaveChangesAsync();
        }

        public async Task UpdatePayment(PaymentCUDTO paymentDTO, int id)
        {
            var payment = await db.Payment.FirstOrDefaultAsync(x => x.PaymentId == id);
            if (payment == null)
            {

                throw new Exception("Payment doesnt found for the Update");
            }


            if (paymentDTO.Amount <= 0)
            {
                throw new ArgumentException("Payment must be Greater than Zero");
            }

            decimal totalPayableAmount;
            switch (paymentDTO.MasterType)
            {

                case "APInvoice":
                    {
                        var invoice = await db.APInvoice.FirstOrDefaultAsync(x => x.APInvoiceId == paymentDTO.MasterId);

                        if (invoice == null)
                        {
                            throw new Exception("AP Invoice not found.");
                        }

                        totalPayableAmount = invoice.InvoiceAmount;
                        break;
                    }

                case "ARInvoice":
                    {
                        var invoice = await db.ARInvoice.FirstOrDefaultAsync(x => x.ARInvoiceId == paymentDTO.MasterId);

                        if (invoice == null)
                        {
                            throw new Exception("AR Invoice not found.");
                        }

                        totalPayableAmount = invoice.InvoiceAmount;
                        break;
                    }

                case "ExpenseClaim":
                    {
                        var expenseClaim = await db.ExpenseClaim.FirstOrDefaultAsync(x => x.ExpenseClaimId == paymentDTO.MasterId);

                        if (expenseClaim == null)
                        {
                            throw new Exception("Expense Claim not found.");
                        }

                        totalPayableAmount = expenseClaim.ExpenseAmount;
                        break;
                    }

                default:
                    throw new ArgumentException("MasterType is Invalid");


            }

            decimal alreadyPaid = await db.Payment.Where(x => x.MasterId == paymentDTO.MasterId && x.MasterType == paymentDTO.MasterType && x.PaymentId != paymentDTO.PaymentId).SumAsync(x => (decimal?)x.Amount) ?? 0;

            decimal remaingAmount = totalPayableAmount - alreadyPaid;

            if (remaingAmount <= 0)
            {
                throw new Exception("Payment has been completed Already");
            }

            if (remaingAmount < paymentDTO.Amount)
            {
                throw new Exception("Payment Can be exceeed than Remaning Invoice Amount");
            }



            payment.MasterId = paymentDTO.MasterId;
            payment.MasterType = paymentDTO.MasterType;
            payment.Amount = paymentDTO.Amount;
            payment.PaymentDate = paymentDTO.PaymentDate;
            payment.TransactionType = paymentDTO.TransactionType;
            payment.PaymentMode = paymentDTO.PaymentMode;
            payment.Remarks = paymentDTO.Remarks;

            payment.ModifiedBy = paymentDTO.ModifiedBy;
            payment.ModifiedAt = DateTime.Now;

            await db.SaveChangesAsync();
        }

        public async Task<bool> DeletePaymentById(int id)
        {
            var payment = await db.Payment.FirstOrDefaultAsync(x => x.PaymentId == id);
            if (payment == null)
            {
                throw new Exception($"Payment {id} was not found.");
            }

            db.Payment.Remove(payment);
            await db.SaveChangesAsync();
            return true;

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

        public async Task<List<PaymentMasterSearchDTO>> SearchByMasterType(SearchByMasterTypeReqDTO dto)
        {

            switch (dto.MasterType)
            {
                case "APInvoice":
                    return await searchAPInvoice(dto.SearchText);

                case "ARInvoice":
                    return await searchARInvoice(dto.SearchText);

                case "ExpenseClaim":
                    return await searchExpenseClaim(dto.SearchText);

                default:
                    throw new Exception("Invalid Master Type has been selected");
            }

        }

        private async Task<List<PaymentMasterSearchDTO>> searchAPInvoice(string? searchText)
        {

            var query = db.APInvoice.Where(x => x.Status != "Paid");

            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim();
                query = query.Where(x => x.InvoiceNumber == searchText);
            }

            var data = await query.OrderByDescending(x => x.InvoiceDate).Take(20).
                Select(x => new PaymentMasterSearchDTO
                {

                    MasterId = x.APInvoiceId,
                    MasterType = "APInvoice",
                    DocumentNumber = x.InvoiceNumber,
                    DocumentDate = x.InvoiceDate,
                    TotalAmount = x.InvoiceAmount,
                    PartyName = x.Vendor.VendorName,
                    PaymentDirection = "Debit"

                }).ToListAsync();

            return data;

        }

        private async Task<List<PaymentMasterSearchDTO>> searchARInvoice(string? searchText)
        {

            var query = db.ARInvoice.Where(x => x.Status != "Paid");

            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim();
                query = query.Where(x => x.InvoiceNumber == searchText);
            }

            var data = await query.OrderByDescending(x => x.InvoiceDate).Take(20).
                Select(x => new PaymentMasterSearchDTO
                {

                    MasterId = x.ARInvoiceId,
                    MasterType = "ARInvoice",
                    DocumentNumber = x.InvoiceNumber,
                    DocumentDate = x.InvoiceDate,
                    TotalAmount = x.InvoiceAmount,
                    PartyName = x.Customer.CustomerName,
                    PaymentDirection = "Credit"

                }).ToListAsync();

            return data;


        }

        private async Task<List<PaymentMasterSearchDTO>> searchExpenseClaim(string? searchText)
        {

            var query = db.ExpenseClaim.Where(x => x.Status != "Paid");

            if (!string.IsNullOrEmpty(searchText))
            {
                searchText = searchText.Trim();
                query = query.Where(x => x.ClaimNumber == searchText);
            }

            var data = await query.OrderByDescending(x => x.ExpenseDate).Take(20).
                Select(x => new PaymentMasterSearchDTO
                {

                    MasterId = x.ExpenseClaimId,
                    MasterType = "ExpenseClaim",
                    DocumentNumber = x.ClaimNumber,
                    DocumentDate = x.ExpenseDate,
                    TotalAmount = x.ExpenseAmount,
                    PartyName = x.ClaimedByUser.Username,
                    PaymentDirection = "Credit"

                }).ToListAsync();

            return data;


        }


    }
}
