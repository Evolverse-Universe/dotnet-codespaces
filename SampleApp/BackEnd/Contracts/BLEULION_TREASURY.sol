// SPDX-License-Identifier: MIT
pragma solidity ^0.8.20;

/**
 * @title BLEULION_TREASURY
 * @dev Revenue vault for BLEU Flame™ NFT ecosystem
 * Implements revenue routing, staking mechanisms, and Harvest-Mint-Heal loop integration
 */
contract BLEULION_TREASURY {
    
    // Treasury state
    address public owner;
    uint256 public totalRevenue;
    uint256 public totalStaked;
    
    // Revenue split configuration (basis points: 1% = 100)
    uint256 public constant PUBLIC_DROP_BPS = 6000;      // 60%
    uint256 public constant ELITE_FOUNDERS_BPS = 3000;   // 30%
    uint256 public constant GOD_TIER_BPS = 1000;         // 10%
    
    // π⁴ constant for yield calculations (scaled by 1e18)
    // π⁴ = π * π * π * π ≈ 97.409091034
    uint256 public constant PI_FOURTH = 97409091034000000000; // 97.409091034 * 1e18
    
    // Revenue tracking by tier
    mapping(uint8 => uint256) public tierRevenue;
    
    // Staking balances
    mapping(address => uint256) public stakedBalances;
    mapping(address => uint256) public lastStakeTime;
    
    // Vault events tracking
    struct VaultEvent {
        string tokenId;
        uint256 temperature;
        uint256 memoryIndex;
        uint8 ownershipTier;
        uint256 calculatedYield;
        uint256 timestamp;
    }
    
    VaultEvent[] public vaultEvents;
    
    // Income sectors
    enum IncomeSector {
        SmartCeramics,
        MetaCuisine,
        ENFTCollectibles,
        EnergyTreasury,
        ThermalDataExchange,
        SpaceLogistics
    }
    
    mapping(IncomeSector => uint256) public sectorRevenue;
    
    // Events
    event RevenueDeposited(address indexed from, uint256 amount, uint8 tier);
    event Staked(address indexed staker, uint256 amount);
    event Unstaked(address indexed staker, uint256 amount);
    event YieldCalculated(string tokenId, uint256 yield);
    event SectorRevenueAdded(IncomeSector sector, uint256 amount);
    
    modifier onlyOwner() {
        require(msg.sender == owner, "Only owner can call this function");
        _;
    }
    
    constructor() {
        owner = msg.sender;
    }
    
    /**
     * @dev Deposit revenue with tier allocation
     */
    function depositRevenue(uint8 tier) external payable {
        require(tier >= 1 && tier <= 3, "Invalid tier");
        require(msg.value > 0, "Amount must be greater than 0");
        
        tierRevenue[tier] += msg.value;
        totalRevenue += msg.value;
        
        emit RevenueDeposited(msg.sender, msg.value, tier);
    }
    
    /**
     * @dev Record vault event and calculate yield
     * Yield = (Temperature × MemoryIndex × OwnershipTier) / π⁴
     */
    function recordVaultEvent(
        string memory tokenId,
        uint256 temperature,
        uint256 memoryIndex,
        uint8 ownershipTier
    ) external returns (uint256) {
        require(ownershipTier >= 1 && ownershipTier <= 3, "Invalid ownership tier");
        
        // Calculate tier multiplier
        uint256 tierMultiplier;
        if (ownershipTier == 1) tierMultiplier = 1e18;        // 1.0
        else if (ownershipTier == 2) tierMultiplier = 2e18;   // 2.0
        else tierMultiplier = 5e18;                            // 5.0
        
        // Calculate yield: (temp * memIndex * tierMultiplier) / π⁴
        // Note: All values are scaled by 1e18, so we divide by PI_FOURTH
        uint256 calculatedYield = (temperature * memoryIndex * tierMultiplier) / PI_FOURTH;
        
        // Store vault event
        vaultEvents.push(VaultEvent({
            tokenId: tokenId,
            temperature: temperature,
            memoryIndex: memoryIndex,
            ownershipTier: ownershipTier,
            calculatedYield: calculatedYield,
            timestamp: block.timestamp
        }));
        
        emit YieldCalculated(tokenId, calculatedYield);
        
        return calculatedYield;
    }
    
    /**
     * @dev Stake EGoin tokens in Energy Treasury
     */
    function stake() external payable {
        require(msg.value > 0, "Must stake positive amount");
        
        stakedBalances[msg.sender] += msg.value;
        lastStakeTime[msg.sender] = block.timestamp;
        totalStaked += msg.value;
        
        emit Staked(msg.sender, msg.value);
    }
    
    /**
     * @dev Unstake tokens with compound interest at π⁴ rate
     */
    function unstake(uint256 amount) external {
        require(stakedBalances[msg.sender] >= amount, "Insufficient staked balance");
        
        // Calculate compound interest (simplified daily compound)
        uint256 timeStaked = block.timestamp - lastStakeTime[msg.sender];
        uint256 daysStaked = timeStaked / 1 days;
        
        // Compound yield = principal * (1 + rate)^days
        // For simplicity, linear approximation: yield = principal * days * rate
        uint256 yield = (amount * daysStaked * PI_FOURTH) / (365 * 1e18);
        uint256 totalAmount = amount + yield;
        
        stakedBalances[msg.sender] -= amount;
        totalStaked -= amount;
        
        payable(msg.sender).transfer(totalAmount);
        
        emit Unstaked(msg.sender, totalAmount);
    }
    
    /**
     * @dev Add revenue from cross-sector income routes
     */
    function addSectorRevenue(IncomeSector sector) external payable {
        require(msg.value > 0, "Amount must be greater than 0");
        
        sectorRevenue[sector] += msg.value;
        totalRevenue += msg.value;
        
        emit SectorRevenueAdded(sector, msg.value);
    }
    
    /**
     * @dev Get total vault events count
     */
    function getVaultEventsCount() external view returns (uint256) {
        return vaultEvents.length;
    }
    
    /**
     * @dev Get revenue split for a given amount
     */
    function getRevenueSplit(uint256 amount) external pure returns (
        uint256 publicDrop,
        uint256 eliteFounders,
        uint256 godTier
    ) {
        publicDrop = (amount * PUBLIC_DROP_BPS) / 10000;
        eliteFounders = (amount * ELITE_FOUNDERS_BPS) / 10000;
        godTier = (amount * GOD_TIER_BPS) / 10000;
    }
    
    /**
     * @dev Withdraw funds (owner only, for authorized distributions)
     */
    function withdraw(uint256 amount) external onlyOwner {
        require(address(this).balance >= amount, "Insufficient balance");
        payable(owner).transfer(amount);
    }
    
    /**
     * @dev Get contract balance
     */
    function getBalance() external view returns (uint256) {
        return address(this).balance;
    }
}
