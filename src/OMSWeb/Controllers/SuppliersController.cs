﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OMSWeb.Dto.Model.SupplierDto;
using OMSWeb.Queries.Queries;
using OMSWeb.Services.Maps;
using OMSWeb.Services.Pagination;
using OMSWeb.Wrappers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace OMSWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SuppliersController : ControllerBase
    {
        private readonly ISupplierQueryProcessor queryProcessor;
        private readonly IAutoMapper autoMapper;
        private readonly IUriService uriService;

        public SuppliersController(ISupplierQueryProcessor queryProcessor, IAutoMapper autoMapper, IUriService uriService)
        {
            this.queryProcessor = queryProcessor;
            this.autoMapper = autoMapper;
            this.uriService = uriService;
        }

        // GET: api/Suppliers
        [HttpGet]
        public async Task<IActionResult> GetSuppliers([FromQuery] uint pageNumber, [FromQuery] uint pageSize)
        {
            var validPaginationInfo = new PaginationInfo(pageSize, pageNumber);
            var route = Request.Path.Value;

            var query = queryProcessor.Get();

            var pagedDataQuery = query
                .Skip(((int)validPaginationInfo.PageNumber - 1) * (int)validPaginationInfo.PageSize)
                .Take((int)validPaginationInfo.PageSize);

            var pagedData = await pagedDataQuery.ToListAsync();

            var resultCollection = autoMapper.Map<List<DtoSupplierGet>>(pagedData);

            var totalRecords = await query.CountAsync();
            var pagedReponse = PaginationHelper.CreatePagedReponse<DtoSupplierGet>(resultCollection, validPaginationInfo, totalRecords, uriService, route);

            return Ok(pagedReponse);
        }

        // GET: api/Suppliers/5
        [HttpGet("{id}")]
        public async Task<DtoSupplierGet> GetSupplier(int id)
        {
            var item = await queryProcessor.GetById(id);
            var model = autoMapper.Map<DtoSupplierGet>(item);
            return model;
        }

        // PUT: api/Suppliers/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<DtoSupplierGet> PutSupplier(int id, [FromBody] DtoSupplierPut dtoSupplier)
        {
            var item = await queryProcessor.Update(id, dtoSupplier);
            var model = autoMapper.Map<DtoSupplierGet>(item);
            return model;
        }

        // POST: api/Suppliers
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<DtoSupplierGet> PostSupplier([FromBody] DtoSupplierPost dtoSupplier)
        {
            var item = await queryProcessor.Create(dtoSupplier);

            var product = autoMapper.Map<DtoSupplierGet>(item);

            return product;
        }

        // DELETE: api/Suppliers/5
        [HttpDelete("{id}")]
        public async Task DeleteSupplier(int id)
        {
            await queryProcessor.Delete(id);
        }
    }
}
