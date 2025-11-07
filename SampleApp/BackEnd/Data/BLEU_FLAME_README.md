# BLEU Flame™ Market Tier System

## Overview

The BLEU Flame™ Market Tier is a comprehensive ENFT (Enhanced Non-Fungible Token) system integrated into the BLEU Codex recursive economy. This system combines physical SmartCeramic tokens with digital twin ownership, memory logging, and automated yield generation through the MetaVault integration.

## Architecture

### Core Components

1. **ENFT Class Schema** - Digital twin representation of SmartCeramic plates
2. **Three-Tier System** - PublicDrop (60%), EliteFounders (30%), GodTier (10%)
3. **MetaVault Integration** - Yield calculation and event tracking
4. **BLEULION Treasury** - Revenue vault and staking mechanism
5. **Energy Treasury** - EGoin staking with compound interest at π⁴ rate

## ENFT Tiers

### 1. PublicDrop (60% Distribution)
- **Supply**: 6,000 NFTs
- **Price**: 0.1 ETH
- **Yield Multiplier**: 1.0x
- **Features**:
  - Thermal calibration tracking
  - Memory log recording
  - Basic yield generation
  - Energy Treasury access

### 2. EliteFounders (30% Distribution)
- **Supply**: 3,000 NFTs
- **Price**: 0.5 ETH
- **Yield Multiplier**: 2.0x
- **Features**:
  - Advanced thermal calibration
  - Enhanced memory indexing
  - 2x yield multiplier
  - Priority MetaVault access
  - Founder recognition

### 3. GodTier (10% Distribution)
- **Supply**: 1,000 NFTs (1-of-1 designs)
- **Price**: 2.5 ETH
- **Yield Multiplier**: 5.0x
- **Features**:
  - Master thermal calibration
  - Maximum memory indexing
  - 5x yield multiplier
  - Exclusive MetaVault priority
  - God tier recognition
  - Direct revenue stream access
  - Custom engraving options

## MetaVault Yield Formula

Each plate interaction triggers a vault event that calculates yield using:

```
Yield = (Temperature × MemoryIndex × OwnershipTier) / π⁴
```

Where:
- **Temperature**: Thermal calibration reading (300-500°C typical)
- **MemoryIndex**: Usage tracking metric (0.0-2.0)
- **OwnershipTier**: Multiplier based on tier (1.0x, 2.0x, or 5.0x)
- **π⁴**: 97.409091034 (mathematical constant)

## Cross-Sector Income Routes

| Sector | Mechanism | Output |
|--------|-----------|--------|
| SmartCeramics | Adaptive cookware, industrial heat tiles | Royalties → FlameCoin |
| Meta-Cuisine | Galaxy Grub™ franchising | 25% yield → HarvestCoin |
| ENFT Collectibles | Tiered drops, resale royalties | 7.5% → BLEULION Fund |
| Energy Treasury | EGoin staking | Compound daily @ π⁴ rate |
| Thermal Data Exchange | Sell heat-curve datasets | ScrollCoin |
| Space Logistics | Cargo licensing for smart dishes | Blu-Tillion |

## Revenue Split

All primary sales are split according to the tier distribution:
- **60%** - PublicDrop allocations
- **30%** - EliteFounders allocations
- **10%** - GodTier allocations

Secondary sales include a 7.5% royalty directed to the BLEULION Fund.

## Harvest-Mint-Heal Loop

The system implements a continuous economic cycle:

1. **HARVEST**: Collect thermal data and usage metrics from SmartCeramic plates
2. **MINT**: Generate yield tokens based on vault events
3. **HEAL**: Reinvest portion of yield into Energy Treasury for compound growth

## API Endpoints

### BLEU Flame™ Endpoints

- `GET /bleu/metadata` - Get collection metadata
- `POST /bleu/mint` - Mint a new BLEU Flame™ ENFT
- `GET /bleu/nft/{tokenId}` - Get ENFT by token ID
- `GET /bleu/tier/{tier}` - Get all ENFTs by tier
- `GET /bleu/owner/{owner}` - Get all ENFTs owned by address
- `GET /bleu/stats` - Get registry statistics
- `GET /bleu/revenue-split/{amount}` - Calculate revenue split for amount

### MetaVault Endpoints

- `POST /metavault/trigger` - Trigger vault event for plate interaction
- `GET /metavault/events/{tokenId}` - Get all vault events for token
- `GET /metavault/yield/{tokenId}` - Get total yield for token
- `POST /metavault/stake` - Stake EGoin in Energy Treasury
- `GET /metavault/treasury` - Get Energy Treasury status
- `GET /metavault/stats` - Get vault statistics
- `POST /metavault/harvest-mint-heal` - Execute full H-M-H loop

## Smart Contract

The BLEULION_TREASURY.sol contract manages:
- Revenue deposits with tier allocation
- Vault event recording and yield calculation
- EGoin staking with compound interest
- Cross-sector revenue tracking
- Revenue split calculations

**Contract Address**: `0xBLEULION_TREASURY_CONTRACT_ADDRESS` (to be deployed)

## File Structure

```
BackEnd/
├── Models/BLEU/
│   ├── ENFTMetadata.cs         # ENFT data structures
│   ├── RevenueModels.cs        # Revenue and allocation models
│   └── MetaVaultModels.cs      # Vault events and yield calculations
├── Services/BLEU/
│   ├── BLEUFlameService.cs     # ENFT management service
│   └── MetaVaultService.cs     # Vault and treasury operations
├── Contracts/
│   └── BLEULION_TREASURY.sol   # Solidity smart contract
└── Data/
    ├── BLEU_Flame_Metadata.json # Collection metadata for IPFS
    ├── BLEU_Flame_Ledger.csv    # Transaction ledger
    └── BLEU_FLAME_README.md     # This file
```

## IPFS Integration

### Metadata Structure
Each ENFT has metadata uploaded to IPFS with the following structure:
- Collection information
- Tier specifications
- Attributes and features
- Revenue split configuration
- Cross-sector routes
- MetaVault formula details

### Deployment Steps
1. Generate metadata JSON for each NFT
2. Upload images to IPFS (ipfs://QmBLEUFlame/...)
3. Upload metadata JSON to IPFS
4. Store IPFS hashes in ENFT records
5. Register with BLEULION Treasury contract

## Usage Examples

### Minting an ENFT
```http
POST /bleu/mint
Content-Type: application/json

{
  "tier": "EliteFounders",
  "owner": "0x1234567890abcdef"
}
```

### Triggering a Vault Event
```http
POST /metavault/trigger
Content-Type: application/json

{
  "tokenId": "BLEU-001",
  "temperature": 375.5,
  "memoryIndex": 1.2,
  "tier": "EliteFounders"
}
```

### Staking EGoin
```http
POST /metavault/stake
Content-Type: application/json

{
  "address": "0x1234567890abcdef",
  "amount": 100.0
}
```

## Future Enhancements

1. Full blockchain integration with deployed smart contracts
2. IPFS pinning service for metadata persistence
3. Frontend UI for minting and managing ENFTs
4. Real-time thermal sensor integration
5. Automated Harvest-Mint-Heal loop execution
6. Cross-chain bridge for multi-chain support
7. DAO governance for treasury management

## Support

For questions or issues, please refer to the main repository documentation or contact the BLEU Codex development team.

## Version

Current Version: 1.0.0
Last Updated: 2025-11-03
