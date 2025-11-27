# Blockchain Forensic Analysis System

## Overview

The **Blockchain Forensic Analysis System** provides comprehensive tools for tracking asset movements across multiple blockchain networks, generating legal chain-of-custody documentation, and creating forensic reports suitable for legal proceedings, insurance claims, and recovery efforts.

This system is designed to help users who have experienced asset movement issues—whether due to protocol design, social engineering, contract exploits, or other causes—to document and prove the chain of custody for their assets.

## Key Features

### 🔍 **Cross-Chain Asset Tracking**
Track assets across multiple blockchain networks:
- Ethereum (Etherscan)
- Cronos (CronoScan)
- Avalanche (Snowtrace)
- Base (BaseScan)
- Polygon (PolygonScan)
- BNB Chain (BscScan)
- Arbitrum (Arbiscan)
- Optimism (Optimistic Etherscan)
- Fantom (FtmScan)
- Solana (Solscan)

### 📋 **Contract Event Tracking**
Monitor and analyze key blockchain events:
- **Transfer**: Token movements between addresses
- **Swap**: DEX swap operations (swapExactTokensForETH, swapETHForExactTokens, etc.)
- **Burn**: Token destruction events
- **Mint**: Token creation events
- **Bridge**: Cross-chain asset transfers
- **Deposit/Withdraw**: Protocol interactions
- **Stake/Unstake**: Staking operations

### 🔗 **Chain of Custody Documentation**
Generate legal-grade provenance trails:
- Sequential transaction documentation
- Timestamp verification
- Block height references
- Value impact tracking
- Evidence URLs to block explorers
- Integrity hashes for verification

### 📊 **Forensic Reports**
Generate comprehensive reports including:
- Executive summary
- Chronological timeline of events
- Technical findings with severity ratings
- Legal narrative for court proceedings
- Evidence references with verification hashes
- Recovery recommendations

## API Endpoints

### Case Management

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/forensics/case/create` | POST | Create a new forensic investigation case |
| `/forensics/cases` | GET | List all forensic cases |
| `/forensics/case/{caseId}` | GET | Get case details by ID |

### Transaction Analysis

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/forensics/transaction/record` | POST | Record a transaction for analysis |
| `/forensics/address/{address}/transactions` | GET | Get transactions by address |

### Asset Flow Tracking

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/forensics/flow/trace` | POST | Trace asset movement from source to destination |
| `/forensics/case/{caseId}/flows` | GET | Get all asset flows for a case |

### Chain of Custody

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/forensics/case/{caseId}/chain-of-custody` | GET | Generate chain of custody documentation |

### Forensic Reports

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/forensics/report/generate` | POST | Generate comprehensive forensic report |
| `/forensics/case/{caseId}/export/csv` | GET | Export case data to CSV |

### Specialized Analysis

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/forensics/swap/analyze` | POST | Analyze DEX swap transaction |
| `/forensics/bridge/analyze` | POST | Analyze cross-chain bridge transaction |
| `/forensics/address/{address}/analytics` | GET | Get forensic profile for address |

### Statistics

| Endpoint | Method | Description |
|----------|--------|-------------|
| `/forensics/stats` | GET | Get overall forensic analysis statistics |

## Usage Examples

### Create a Forensic Case

```bash
curl -X POST "http://localhost:8080/forensics/case/create" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "AVAX Token Transfer Investigation",
    "description": "Tracking unauthorized transfer of AVAX tokens from wallet",
    "addresses": ["0x1234...abcd", "0x5678...efgh"],
    "networks": ["Avalanche", "Ethereum"]
  }'
```

### Trace Asset Flow

```bash
curl -X POST "http://localhost:8080/forensics/flow/trace" \
  -H "Content-Type: application/json" \
  -d '{
    "assetSymbol": "AVAX",
    "assetContract": "0x...",
    "sourceAddress": "0x1234...abcd",
    "destinationAddress": "0x5678...efgh",
    "amount": 1000.50,
    "sourceNetwork": "Avalanche",
    "txHash": "0xabc123..."
  }'
```

### Generate Forensic Report

```bash
curl -X POST "http://localhost:8080/forensics/report/generate?caseId=CASE-000001&preparedBy=Forensic%20Analyst"
```

## Forensic Report Structure

### 1. Executive Summary
High-level overview of the investigation including:
- Case identification
- Networks involved
- Total assets tracked
- Total assets potentially lost
- Investigation status

### 2. Timeline of Events
Chronological sequence showing:
- Order of events
- Timestamps (UTC)
- Transaction descriptions
- Network identification
- Value impact
- Explorer URLs for verification

### 3. Asset Flows
Detailed tracking of each asset movement:
- Source and destination addresses
- Amount transferred
- Network information
- Movement status (Transferred, Burned, Bridged, Lost, etc.)
- Forensic narrative

### 4. Chain of Custody (Provenance Trail)
Legal documentation showing:
- Sequence numbers
- Holder addresses with labels
- Action types (Transfer, Swap, Burn, Bridge)
- Value before/after each movement
- Evidence URLs
- Verification status

### 5. Technical Findings
Categorized findings with:
- Severity levels (Critical, High, Medium, Low, Informational)
- Categories (Contract Exploit, Social Engineering, Protocol Design, User Error)
- Evidence references
- Impact assessment
- Affected transactions

### 6. Evidence References
Documented evidence including:
- Transaction hashes
- Block explorer sources
- Capture timestamps
- Integrity hashes for verification

### 7. Recommendations
Action items based on findings:
- Evidence preservation guidance
- Regulatory reporting suggestions
- Recovery options
- Legal consultation recommendations

## Asset Movement Status Types

| Status | Description |
|--------|-------------|
| `InControl` | Asset remains in user's control |
| `Transferred` | Asset transferred to another address |
| `Swapped` | Asset exchanged for another token via DEX |
| `Burned` | Asset sent to burn address (irrecoverable) |
| `Bridged` | Asset transferred to another blockchain |
| `Lost` | Asset sent to mixer/tumbler or wrong address |
| `Recovered` | Asset recovered after incident |
| `Disputed` | Asset movement under dispute |

## Known Address Detection

The system automatically detects:

### Burn Addresses
- `0x0000000000000000000000000000000000000000`
- `0x000000000000000000000000000000000000dEaD`
- `0xdead000000000000000000000000000000000000`

### Known DEX Routers
- Uniswap V2/V3 (Ethereum)
- Trader Joe (Avalanche)
- Additional routers can be configured

### Suspicious Patterns
- Mixer/tumbler contract interactions
- Rapid multi-hop transfers
- Cross-chain bridges with missing destination transactions

## Legal Use Cases

This system is designed to support:

1. **Court Proceedings**: Chain of custody documentation suitable for legal evidence
2. **Insurance Claims**: Documented proof of asset loss with verified blockchain evidence
3. **Regulatory Filings**: Transaction histories and compliance documentation
4. **Recovery Efforts**: Tracing assets through multiple addresses and chains
5. **Arbitration**: Technical evidence for dispute resolution

## Best Practices

### For Evidence Collection
1. Record all transactions as soon as discovered
2. Generate chain of custody documentation
3. Export data to CSV for archival
4. Screenshot block explorer evidence
5. Generate forensic report with findings

### For Legal Proceedings
1. Maintain immutable copies of blockchain evidence
2. Document timestamp of evidence collection
3. Use integrity hashes for verification
4. Reference multiple independent block explorers
5. Engage qualified forensic experts for testimony

## Security Considerations

- All transaction hashes can be independently verified on public block explorers
- Integrity hashes are computed using SHA-256
- No private keys are stored or processed
- All data is derived from public blockchain data
- Reports include verification methods for third-party validation

## Integration with EV0LVerse Ecosystem

This forensic system integrates with:
- **Unified Economic Ledger**: Cross-reference with internal transaction records
- **BLEU Flame™**: Track ENFT movements and ownership changes
- **Zion Gold Bar Protocol**: Verify gold bar token provenance
- **MetaVault**: Audit yield generation and staking events

## Support

For assistance with blockchain forensic investigations:
- Review the API documentation at `/scalar` when running the backend
- Check block explorer evidence URLs in generated reports
- Consult the technical findings for actionable insights
- Engage professional forensic analysts for complex cases

---

*The blockchain is the evidence. This system helps you read it, document it, and present it.*
