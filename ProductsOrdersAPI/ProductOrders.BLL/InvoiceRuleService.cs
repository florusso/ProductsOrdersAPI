using ProductsOrders.DAL.Models;
using ProductsOrders.DAL.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace ProductsOrders.BLL
{
    public class InvoiceRuleService : IInvoiceRuleService
    {
        private readonly IInvoiceRuleRepository _invoiceRuleRepository;

        public InvoiceRuleService(IInvoiceRuleRepository invoiceRuleRepository)
        {
            _invoiceRuleRepository = invoiceRuleRepository;
        }

        public double Apply(int CustomerCode, double summation)
        {
            var rule = _invoiceRuleRepository.Get(CustomerCode).GetAwaiter().GetResult();
            double bill = 0;

            switch (rule.Operator)
            {
                case "+":
                    bill = summation + rule.OperatorValue;
                    break;

                case "*":
                    bill = summation * rule.OperatorValue;
                    break;

                case "%":
                    bill = summation + (summation * (rule.OperatorValue / 100));
                    break;

                default:
                    bill = summation;
                    break;
            }

            return bill;
        }
    }
}