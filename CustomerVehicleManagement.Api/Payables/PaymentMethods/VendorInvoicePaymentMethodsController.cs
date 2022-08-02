﻿using CustomerVehicleManagement.Api.Data;
using CustomerVehicleManagement.Shared.Models.Payables.Invoices.Payments;
using Menominee.Common.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerVehicleManagement.Api.Payables.PaymentMethods
{
    public class VendorInvoicePaymentMethodsController : ApplicationController
    {
        private readonly IVendorInvoicePaymentMethodRepository repository;

        public VendorInvoicePaymentMethodsController(IVendorInvoicePaymentMethodRepository repository)
        {
            this.repository = repository ?? throw new ArgumentNullException(nameof(repository));
        }

        // GET: api/vendorinvoicepaymentmethods/listing
        [Route("listing")]
        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<VendorInvoicePaymentMethodToReadInList>>> GetPaymentMethodListAsync()
        {
            var payMethods = await repository.GetPaymentMethodListAsync();

            if (payMethods == null)
                return NotFound();

            return Ok(payMethods);
        }

        // GET: api/vendorinvoicepaymentmethods/1
        [HttpGet("{id:long}", Name = "GetPaymentMethodAsync")]
        public async Task<ActionResult<VendorInvoicePaymentMethodToRead>> GetPaymentMethodAsync(long id)
        {
            var payMethod = await repository.GetPaymentMethodAsync(id);

            if (payMethod == null)
                return NotFound();

            return Ok(payMethod);
        }

        // PUT: api/vendorinvoicepaymentmethods/1
        [HttpPut("{id:long}")]
        public async Task<IActionResult> UpdatePaymentMethodAsync(VendorInvoicePaymentMethodToWrite payMethodToWrite, long id)
        {
            var notFoundMessage = $"Could not find Vendor Invoice Payment Method to update with Id = {id}.";

            if (!await repository.PaymentMethodExistsAsync(id))
                return NotFound(notFoundMessage);

            var payMethod = repository.GetPaymentMethodEntityAsync(id).Result;
            if (payMethod is null)
                return NotFound(notFoundMessage);

            VendorInvoicePaymentMethodHelper.CopyWriteDtoToEntity(payMethodToWrite, payMethod);

            payMethod.SetTrackingState(TrackingState.Modified);
            repository.FixTrackingState();

            repository.UpdatePaymentMethod(payMethod);
            if (await repository.SaveChangesAsync())
                return NoContent();

            return BadRequest($"Failed to update vendor invoice payment method.  Id = {id}.");
        }

        // POST: api/vendorinvoicepaymentmethods
        [HttpPost]
        public async Task<ActionResult<VendorInvoicePaymentMethodToRead>> AddPaymentMethodAsync(VendorInvoicePaymentMethodToWrite payMethodToAdd)
        {
            var payMethod = VendorInvoicePaymentMethodHelper.ConvertWriteDtoToEntity(payMethodToAdd);

            await repository.AddPaymentMethodAsync(payMethod);
            await repository.SaveChangesAsync();

            return CreatedAtRoute("GetPaymentMethodAsync",
                                  new { id = payMethod.Id },
                                  VendorInvoicePaymentMethodHelper.ConvertEntityToReadDto(payMethod));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeletePaymentMethodAsync(int id)
        {
            var payMethodFromRepository = await repository.GetPaymentMethodAsync(id);

            if (payMethodFromRepository == null)
                return NotFound($"Could not find Vendor Invoice Payment Method in the database to delete with Id: {id}.");

            await repository.DeletePaymentMethodAsync(id);

            if (await repository.SaveChangesAsync())
                return NoContent();

            return BadRequest($"Failed to delete Vendor Invoice Payment Method with Id of {id}.");
        }
    }
}
