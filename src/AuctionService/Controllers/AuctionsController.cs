using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AuctionService.DTOs;
using AuctionService.Data;
using AuctionService.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuctionsController : ControllerBase
    {
        private readonly AuctionDbContext _context;

        private readonly IMapper _mapper;

        public AuctionsController(AuctionDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<AuctionDto>>> GetAllAuctions()
        {
            var auctions =
                await _context
                    .Auctions
                    .Include(x => x.Item)
                    .OrderBy(x => x.Item.Make)
                    .ToListAsync();

            return _mapper.Map<List<AuctionDto>>(auctions);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AuctionDto>> GetAuctionById(Guid id)
        {
            var auction =
                await _context
                    .Auctions
                    .Include(x => x.Item)
                    .FirstOrDefaultAsync(x => x.Id == id);
            if (auction == null) return NotFound();
            return _mapper.Map<AuctionDto>(auction);
        }

        [HttpPost]
        public async Task<ActionResult<AuctionDto>>
        CreateAuction(CreateAuctionDto auctionDto)
        {
            var auction = _mapper.Map<Auction>(auctionDto);

            _context.Auctions.Add (auction);
            var result = await _context.SaveChangesAsync() > 0;

            if (!result) return BadRequest("error");

            return CreatedAtAction(nameof(GetAuctionById),
            new { auction.Id },
            _mapper.Map<Auction>(auction));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AuctionDto>>
        UpdateAuction(Guid id, UpdateAuctionDto auctionDto)
        {
            var auction =
                await _context
                    .Auctions
                    .Include(x => x.Item)
                    .FirstOrDefaultAsync(x => x.Id == id);
            if (auction == null) return NotFound();

            auction.Item.Make = auctionDto.Make ?? auction.Item.Make;
            auction.Item.Model = auctionDto.Model ?? auction.Item.Model;
            auction.Item.Color = auctionDto.Color ?? auction.Item.Color;
            auction.Item.Mileage = auctionDto.Mileage ?? auction.Item.Mileage;
            auction.Item.Year = auctionDto.Year ?? auction.Item.Year;

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest("error");
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult<AuctionDto>> DeleteAuction(Guid id)
        {
            var auction =
                await _context
                    .Auctions
                    .Include(x => x.Item)
                    .FirstOrDefaultAsync(x => x.Id == id);
            if (auction == null) return NotFound();
            _context.Auctions.Remove (auction);

            var result = await _context.SaveChangesAsync() > 0;

            if (result) return Ok();

            return BadRequest("error");
        }
    }
}
