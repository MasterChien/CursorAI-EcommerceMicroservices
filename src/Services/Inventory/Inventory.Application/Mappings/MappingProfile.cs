using AutoMapper;
using Inventory.Application.DTOs;
using Inventory.Core.Entities;
using System;

namespace Inventory.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // InventoryItem mappings
            CreateMap<InventoryItem, InventoryItemDto>()
                .ForMember(dest => dest.AvailableQuantity, opt => opt.MapFrom(src => src.AvailableQuantity))
                .ForMember(dest => dest.IsInStock, opt => opt.MapFrom(src => src.IsInStock))
                .ForMember(dest => dest.IsLowStock, opt => opt.MapFrom(src => src.IsLowStock))
                .ReverseMap();
            CreateMap<CreateInventoryItemDto, InventoryItem>()
                .ForMember(dest => dest.ReservedQuantity, opt => opt.MapFrom(src => 0))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => DateTime.UtcNow));
            CreateMap<UpdateInventoryItemDto, InventoryItem>();
            CreateMap<InventoryItem, InventoryItemWithTransactionsDto>()
                .ForMember(dest => dest.AvailableQuantity, opt => opt.MapFrom(src => src.AvailableQuantity))
                .ForMember(dest => dest.IsInStock, opt => opt.MapFrom(src => src.IsInStock))
                .ForMember(dest => dest.IsLowStock, opt => opt.MapFrom(src => src.IsLowStock));

            // InventoryTransaction mappings
            CreateMap<InventoryTransaction, InventoryTransactionDto>()
                .ForMember(dest => dest.InventoryItemName, opt => opt.MapFrom(src => src.InventoryItem.ProductName))
                .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => src.TransactionType.ToString()));
            CreateMap<CreateInventoryTransactionDto, InventoryTransaction>();

            // Warehouse mappings
            CreateMap<Warehouse, WarehouseDto>();
            CreateMap<CreateWarehouseDto, Warehouse>();
            CreateMap<UpdateWarehouseDto, Warehouse>();
            CreateMap<Warehouse, WarehouseWithItemsDto>();

            // WarehouseItem mappings
            CreateMap<WarehouseItem, WarehouseItemDto>()
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse.Name))
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.InventoryItem.ProductName))
                .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.InventoryItem.SKU))
                .ForMember(dest => dest.AvailableQuantity, opt => opt.MapFrom(src => src.AvailableQuantity));
            CreateMap<CreateWarehouseItemDto, WarehouseItem>()
                .ForMember(dest => dest.ReservedQuantity, opt => opt.MapFrom(src => 0));
            CreateMap<UpdateWarehouseItemDto, WarehouseItem>();

            // InventoryCount mappings
            CreateMap<InventoryCount, InventoryCountDto>()
                .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));
            CreateMap<CreateInventoryCountDto, InventoryCount>()
                .ForMember(dest => dest.CountDate, opt => opt.MapFrom(src => DateTime.UtcNow))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => InventoryCountStatus.Draft))
                .ForMember(dest => dest.CountNumber, opt => opt.MapFrom(src => $"IC-{DateTime.UtcNow:yyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}"));

            // InventoryCountItem mappings
            CreateMap<InventoryCountItem, InventoryCountItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.InventoryItem.ProductName))
                .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.InventoryItem.SKU))
                .ForMember(dest => dest.QuantityDifference, opt => opt.MapFrom(src => src.QuantityDifference));

            // StockTransfer mappings
            CreateMap<StockTransfer, StockTransferDto>()
                .ForMember(dest => dest.SourceWarehouseName, opt => opt.MapFrom(src => src.SourceWarehouse.Name))
                .ForMember(dest => dest.DestinationWarehouseName, opt => opt.MapFrom(src => src.DestinationWarehouse.Name))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()));

            // StockTransferItem mappings
            CreateMap<StockTransferItem, StockTransferItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.InventoryItem.ProductName))
                .ForMember(dest => dest.SKU, opt => opt.MapFrom(src => src.InventoryItem.SKU));
            CreateMap<CreateStockTransferItemDto, StockTransferItem>();
        }
    }
} 