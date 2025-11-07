# API Testing Examples

This document provides example API calls for testing the BLEU Flame™ and Zion Gold Bar protocols.

## Prerequisites

Start the backend server:
```bash
cd SampleApp/BackEnd
dotnet run --urls "http://localhost:8081"
```

Or access via Scalar at: `http://localhost:8081/scalar`

---

## BLEU Flame™ API Examples

### 1. Get Collection Metadata
```bash
curl http://localhost:8081/bleu/metadata
```

**Response (sample)**:
```json
{
  "name": "BLEU Flame™ Collection",
  "description": "BLEU Flame™ SmartCeramic ENFT Collection...",
  "tiers": [...],
  "revenue_split": {...}
}
```

### 2. Mint a BLEU Flame™ ENFT
```bash
curl -X POST "http://localhost:8081/bleu/mint?tier=EliteFounders&owner=0x1234567890abcdef"
```

**Response**:
```json
{
  "tokenId": "BLEU-000001",
  "tier": 2,
  "ownership": {
    "currentOwner": "0x1234567890abcdef",
    "originalMinter": "0x1234567890abcdef"
  }
}
```

**Available Tiers**:
- `PublicDrop` (1) - 60% distribution
- `EliteFounders` (2) - 30% distribution
- `GodTier` (3) - 10% distribution

### 3. Get ENFT by Token ID
```bash
curl http://localhost:8081/bleu/nft/BLEU-000001
```

### 4. Get ENFTs by Owner
```bash
curl http://localhost:8081/bleu/owner/0x1234567890abcdef
```

### 5. Get Registry Statistics
```bash
curl http://localhost:8081/bleu/stats
```

**Response**:
```json
{
  "total": 5,
  "publicDrop": 3,
  "eliteFounders": 1,
  "godTier": 1
}
```

### 6. Calculate Revenue Split
```bash
curl http://localhost:8081/bleu/revenue-split/1000
```

**Response**:
```json
{
  "totalAmount": 1000,
  "publicDrop": 600,
  "eliteFounders": 300,
  "godTier": 100
}
```

---

## MetaVault API Examples

### 1. Trigger Vault Event
```bash
curl -X POST "http://localhost:8081/metavault/trigger?tokenId=BLEU-000001&temperature=375.5&memoryIndex=1.5&tier=EliteFounders"
```

**Response**:
```json
{
  "eventId": "...",
  "tokenId": "BLEU-000001",
  "temperature": 375.5,
  "memoryIndex": 1.5,
  "ownershipTier": 2,
  "calculatedYield": 11.564629009902193
}
```

**Formula**: `Yield = (Temperature × MemoryIndex × OwnershipTier) / π⁴`
- π⁴ = 97.409091034
- Tier multipliers: PublicDrop=1.0x, EliteFounders=2.0x, GodTier=5.0x

### 2. Get Vault Events for Token
```bash
curl http://localhost:8081/metavault/events/BLEU-000001
```

### 3. Get Total Yield for Token
```bash
curl http://localhost:8081/metavault/yield/BLEU-000001
```

**Response**:
```json
{
  "tokenId": "BLEU-000001",
  "totalYield": 48.522165026767
}
```

### 4. Stake EGoin in Energy Treasury
```bash
curl -X POST "http://localhost:8081/metavault/stake?address=0x1234567890abcdef&amount=100"
```

**Response**:
```json
{
  "message": "Staked successfully",
  "address": "0x1234567890abcdef",
  "amount": 100
}
```

### 5. Get Energy Treasury Status
```bash
curl http://localhost:8081/metavault/treasury
```

**Response**:
```json
{
  "totalStaked": 103.69575361168645,
  "compoundRate": 97.409091034,
  "lastCompoundTime": "2025-11-03T...",
  "stakerBalances": {...}
}
```

### 6. Get Vault Statistics
```bash
curl http://localhost:8081/metavault/stats
```

### 7. Execute Harvest-Mint-Heal Loop
```bash
curl -X POST "http://localhost:8081/metavault/harvest-mint-heal?tokenId=BLEU-000001&temperature=400&memoryIndex=1.8&tier=GodTier"
```

**Response**:
```json
{
  "message": "Harvest-Mint-Heal loop completed for BLEU-000001. Yield: 36.957536, Reinvested: 3.695754"
}
```

**Loop Explanation**:
- **HARVEST**: Collects thermal data and usage metrics
- **MINT**: Generates yield based on vault formula
- **HEAL**: Reinvests 10% of yield into Energy Treasury

---

## Zion Gold Bar API Examples

### 1. Get Zion Gold Bar Collection Metadata
```bash
curl http://localhost:8081/zion/metadata
```

### 2. Mint Zion Gold Bar ENFT
```bash
curl -X POST "http://localhost:8081/zion/mint?creatorAddress=0xCreator123&imageCID=QmTestImage123&certificateCID=QmTestCert456"
```

**Response**:
```json
{
  "tokenId": "ZION-000001",
  "name": "Zion Gold Bar ENFT",
  "layers": {
    "goldBar": {...},
    "silverRing": {...},
    "pyroRing": {...},
    "medicinalMineral": {...},
    "shellCoin": {...},
    "es0ilSubstrate": {...}
  },
  "resources": {
    "primaryMetal": "Gold",
    "weight": 1.0,
    "purity": 99.99
  }
}
```

### 3. Get Zion Gold Bar by Token ID
```bash
curl http://localhost:8081/zion/bar/ZION-000001
```

### 4. Get Saturn-Strata Layers
```bash
curl http://localhost:8081/zion/layers
```

**Response**:
```json
{
  "goldBar": {
    "name": "Gold Bar",
    "description": "Foundation layer - Primary wealth store",
    "stackPosition": 1
  },
  "silverRing": {
    "name": "Silver Ring",
    "description": "Lunar resonance - Secondary value circuit",
    "stackPosition": 2
  },
  ...
}
```

### 5. Calculate Zion Gold Bar Yield
```bash
curl http://localhost:8081/zion/yield/ZION-000001
```

**Response**:
```json
{
  "tokenId": "ZION-000001",
  "yield": 0.06158973393875474,
  "formula": "Yield = (Weight × Purity × LayerStack) / π⁴"
}
```

**Formula**: `Yield = (Weight × Purity × LayerStack) / π⁴`
- Weight: 1.0 troy ounce
- Purity: 99.99% (0.9999)
- LayerStack: 6 (six Saturn-Strata layers)
- π⁴: 97.409091034

### 6. Generate Mint-Ready Metadata
```bash
curl -X POST "http://localhost:8081/zion/generate-metadata?tokenId=ZION-000001&imageCID=QmNewImage&certificateCID=QmNewCert&creatorAddress=0xCreator"
```

**Use Case**: Generates JSON metadata ready for IPFS upload and blockchain minting.

### 7. Verify Certificate Authenticity
```bash
curl -X POST "http://localhost:8081/zion/verify-certificate?tokenId=ZION-000001&certificateCID=QmTestCert456"
```

**Response**:
```json
{
  "tokenId": "ZION-000001",
  "certificateCID": "QmTestCert456",
  "isValid": true
}
```

### 8. Get Zion Registry Statistics
```bash
curl http://localhost:8081/zion/stats
```

**Response**:
```json
{
  "totalMinted": 3,
  "totalWeight": 3.0,
  "totalValue": "3.0 troy ounces @ 99.99% purity"
}
```

### 9. Update IPFS CIDs
```bash
curl -X PUT "http://localhost:8081/zion/update-cids?tokenId=ZION-000001&imageCID=QmUpdatedImage&certificateCID=QmUpdatedCert"
```

**Response**:
```json
{
  "message": "IPFS CIDs updated successfully",
  "tokenId": "ZION-000001"
}
```

---

## Integration Examples

### Complete BLEU Flame™ Workflow

1. **Mint an ENFT**:
```bash
TOKEN_ID=$(curl -s -X POST "http://localhost:8081/bleu/mint?tier=GodTier&owner=0xMyAddress" | jq -r '.tokenId')
echo "Minted: $TOKEN_ID"
```

2. **Trigger Vault Event**:
```bash
curl -X POST "http://localhost:8081/metavault/trigger?tokenId=$TOKEN_ID&temperature=450&memoryIndex=2.0&tier=GodTier"
```

3. **Check Total Yield**:
```bash
curl "http://localhost:8081/metavault/yield/$TOKEN_ID"
```

4. **Execute H-M-H Loop**:
```bash
curl -X POST "http://localhost:8081/metavault/harvest-mint-heal?tokenId=$TOKEN_ID&temperature=475&memoryIndex=2.2&tier=GodTier"
```

### Complete Zion Gold Bar Workflow

1. **Upload assets to IPFS** (external step):
```bash
# Upload image
ipfs add visual_stack_image.png
# Returns: QmXXXXXXXXXXXX

# Upload certificate
ipfs add certificate.pdf
# Returns: QmYYYYYYYYYYYY
```

2. **Mint Zion Gold Bar**:
```bash
curl -X POST "http://localhost:8081/zion/mint?creatorAddress=0xMyWallet&imageCID=QmXXXXXXXXXXXX&certificateCID=QmYYYYYYYYYYYY"
```

3. **Generate complete metadata**:
```bash
curl -X POST "http://localhost:8081/zion/generate-metadata?tokenId=ZION-000001&imageCID=QmXXXXXXXXXXXX&certificateCID=QmYYYYYYYYYYYY&creatorAddress=0xMyWallet" > metadata.json
```

4. **Upload metadata to IPFS**:
```bash
ipfs add metadata.json
# Returns: QmZZZZZZZZZZZZ
```

5. **Mint on blockchain** (external step - use metadata CID)

---

## Testing with jq

### Pretty Print Response
```bash
curl http://localhost:8081/bleu/metadata | jq '.'
```

### Extract Specific Fields
```bash
curl http://localhost:8081/bleu/stats | jq '.total, .godTier'
```

### Filter Array Results
```bash
curl http://localhost:8081/bleu/tier/GodTier | jq '.[].tokenId'
```

---

## Error Handling

### Invalid Tier
```bash
curl -X POST "http://localhost:8081/bleu/mint?tier=InvalidTier&owner=0x123"
# Returns: 400 Bad Request
```

### Token Not Found
```bash
curl http://localhost:8081/bleu/nft/BLEU-999999
# Returns: 404 Not Found
```

### Invalid Stake Amount
```bash
curl -X POST "http://localhost:8081/metavault/stake?address=0x123&amount=-100"
# Returns: 400 Bad Request with error message
```

---

## Performance Testing

### Bulk Minting (Bash Script)
```bash
#!/bin/bash
for i in {1..100}; do
  curl -s -X POST "http://localhost:8081/bleu/mint?tier=PublicDrop&owner=0xBulkMinter$i" &
done
wait
curl http://localhost:8081/bleu/stats
```

---

## Notes

- All timestamps are in UTC (ISO 8601 format)
- Token IDs are sequential: BLEU-000001, BLEU-000002, etc.
- Zion Gold Bar IDs: ZION-000001, ZION-000002, etc.
- IPFS CIDs should start with "Qm" (standard IPFS v0 format)
- Ethereum addresses should be 42 characters starting with "0x"

---

**Last Updated**: November 3, 2025  
**API Version**: 1.0.0  
**Backend Port**: 8081 (default)
