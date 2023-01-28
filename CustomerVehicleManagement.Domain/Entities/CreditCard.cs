﻿using CSharpFunctionalExtensions;
using Menominee.Common.Enums;
using System;
using Entity = Menominee.Common.Entity;

namespace CustomerVehicleManagement.Domain.Entities
{
    public class CreditCard : Entity
    {
        private const int MinimumLength = 2;
        private const int MaximumLength = 50;
        private const string RequiredMessage = "A valid value is required.";
        private const string InvalidLengthMessage = "Value must be between 2 and 50 characters.";

        public string Name { get; private set; }
        public CreditCardFeeType FeeType { get; private set; }
        public double Fee { get; private set; }
        public bool? IsAddedToDeposit { get; private set; }

        //public virtual CreditCardProcessor Processor { get; set; }

        private CreditCard(
            string name,
            CreditCardFeeType feeType,
            double fee,
            bool? isAddedToDeposit)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentNullException(RequiredMessage);

            name = (name ?? string.Empty).Trim();

            if (name.Length < MinimumLength ||
                name.Length > MaximumLength)
                throw new ArgumentOutOfRangeException(InvalidLengthMessage);

            if (!Enum.IsDefined(typeof(CreditCardFeeType), feeType))
                throw new ArgumentNullException(RequiredMessage);

            Name = name;
            FeeType = feeType;
            Fee = fee;
            IsAddedToDeposit = isAddedToDeposit;
        }

        public static Result<CreditCard> Create(
            string name,
            CreditCardFeeType feeType,
            double fee,
            bool? isAddedToDeposit)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure<CreditCard>(RequiredMessage);

            name = (name ?? string.Empty).Trim();

            if (name.Length < MinimumLength ||
                name.Length > MaximumLength)
                return Result.Failure<CreditCard>(InvalidLengthMessage);

            if (!Enum.IsDefined(typeof(CreditCardFeeType), feeType))
                return Result.Failure<CreditCard>(RequiredMessage);
            
            return Result.Success(new CreditCard(name, feeType, fee, isAddedToDeposit));
        }

        public Result<string> SetName(string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result.Failure<string>(RequiredMessage);

            name = (name ?? string.Empty).Trim();

            if (name.Length < MinimumLength ||
                name.Length > MaximumLength)
                return Result.Failure<string>(InvalidLengthMessage);

            return Result.Success(Name = name);
        }

        public Result<CreditCardFeeType> SetFeeType(CreditCardFeeType feeType)
        {
            if (!Enum.IsDefined(typeof(CreditCardFeeType), feeType))
                return Result.Failure<CreditCardFeeType>("Invalid Fee Type");

            return Result.Success(FeeType = feeType);
        }

        public Result<double> SetFee(double fee)
        {
            return Result.Success(Fee = fee);
        }

        public Result<bool?> SetIsAddedToDeposit(bool? isAddedToDeposit)
        {
            return Result.Success(IsAddedToDeposit = isAddedToDeposit);
        }

        #region ORM

        // EF requires a parameterless constructor
        protected CreditCard() { }

        #endregion
    }
}