﻿using CustomerVehicleManagement.Shared.Models.RepairOrders;
using CustomerVehicleManagement.Shared.Models.RepairOrders.Payments;
using Menominee.Common.Enums;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;

namespace Menominee.Client.Components.RepairOrders
{
    public partial class RepairOrderPaymentEdit : ComponentBase
    {
        [Parameter]
        public RepairOrderPaymentToWrite Payment { get; set; }

        [CascadingParameter]
        public RepairOrderToWrite RepairOrder { get; set; }

        [Parameter]
        public bool DialogVisible { get; set; }

        [Parameter]
        public FormMode Mode
        {
            get => formMode;
            set
            {
                formMode = value;
                if (formMode == FormMode.Add)
                    Title = "Add";
                else if (formMode == FormMode.Edit)
                    Title = "Edit";
                else
                    Title = "View";
                Title += " Payment";
            }
        }

        [Parameter]
        public EventCallback OnSave { get; set; }

        [Parameter]
        public EventCallback OnCancel { get; set; }

        private PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
        private double Amount { get; set; } = 0.0;

        private void Save()
        {
            OnSave.InvokeAsync();

            if (formMode == FormMode.Edit)
            {
                PaymentMethod = PaymentMethod.Cash;
                Amount = 0.0;
            }
        }

        private void Cancel()
        {
            if (formMode == FormMode.Edit)
            {
                Payment.PaymentMethod = PaymentMethod;
                Payment.Amount = Amount;
            }

            OnCancel.InvokeAsync();
        }

        protected override void OnInitialized()
        {
            foreach (PaymentMethod item in Enum.GetValues(typeof(PaymentMethod)))
            {
                PayTypeEnumData.Add(new PayTypeEnumModel { DisplayText = item.ToString(), Value = item });
            }

            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            PaymentMethod = Payment.PaymentMethod;
            Amount = Payment.Amount;

            if (formMode == FormMode.Add)
                Payment.Amount = RepairOrder.Total;

            base.OnParametersSet();
        }

        private FormMode formMode;
        private string Title { get; set; }
        List<PayTypeEnumModel> PayTypeEnumData { get; set; } = new List<PayTypeEnumModel>();
    }

    public class PayTypeEnumModel
    {
        public PaymentMethod Value { get; set; }
        public string DisplayText { get; set; }
    }
}
