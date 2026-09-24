# 🛡️ CloudOps Simulator — Interactive Cloud Architecture & Cyber Defense Simulation

**CloudOps Simulator** is an educational simulation game developed in Unity (C#) as a Final Year Project (FYP). The project gamifies distributed cloud systems and cybersecurity defense concepts, allowing players to design, scale, and protect virtual cloud topologies against dynamic traffic loads, server bottlenecks, and cyber attacks in real time.

---

## 📋 Table of Contents
- [Prerequisites](#-prerequisites)
- [How to Clone and Open the Project](#-how-to-clone-and-open-the-project)
- [Controls & How to Play](#-controls--how-to-play)
- [Core Architecture & Nodes](#-core-architecture--nodes)
- [Telemetry & Game Metrics](#-telemetry--game-metrics)
- [Project Structure](#-project-structure)

---

## ⚙️ Prerequisites

Before getting started, make sure you have the following installed:

- **Git:** [Download Git](https://git-scm.com/)
- **Unity Hub:** [Download Unity Hub](https://unity.com/download)
- **Unity Editor Version:** `6000.3.8f1` (Unity 6)
  - Ensure you have the **Universal Render Pipeline (URP)** and **Windows/Mac Build Support** installed if you plan to build executables.

---

## 🚀 How to Clone and Open the Project

### 1. Clone the Repository
Open your terminal (PowerShell, Command Prompt, or Bash) and clone the repository:

```bash
git clone https://github.com/Muhammad0149/CloudOpsSimulator.git
cd CloudOpsSimulator
```

### 2. Add Project to Unity Hub
1. Launch **Unity Hub**.
2. Click on the **Projects** tab, then click **Add** (or **Add project from disk**).
3. Browse to and select the cloned repository folder (`CloudOpsSimulator`).
4. Ensure the editor version is set to **`6000.3.8f1`** (or compatible Unity 6).

### 3. Open and Run the Game
1. In Unity Hub, click on the project name to open it in the Unity Editor (initial package import may take 1–2 minutes).
2. In the **Project** window at the bottom, navigate to:
   ```
   Assets > Scenes > SampleScene.unity
   ```
3. Double-click **`SampleScene.unity`** to load the main simulation scene.
4. Press the **Play button (▶️)** at the top of the Unity Editor to start the simulation.

---

## 🎮 Controls & How to Play

### Node Deployment
Hover your mouse over the game plane and press the corresponding number key to spawn nodes:

| Key | Node Type | Description |
| :--- | :--- | :--- |
| **`1`** | **Traffic Generator** | Spawns clients generating legitimate and malicious network requests. |
| **`2`** | **Load Balancer** | Distributes incoming network requests across downstream nodes (Round Robin). |
| **`3`** | **Compute Node** | Processes incoming requests and generates revenue upon successful processing. |
| **`4`** | **Firewall** | Inspects payloads and filters out malicious traffic before reaching servers. |

### Network Wiring & Routing
- **Connect Nodes:** **Left-Click** on a source node to select it, then **Left-Click** on a target node to establish a directional network cable connection.
- **Cancel Selection:** **Right-Click** anywhere to cancel the current node selection.

---

## 🧩 Core Architecture & Nodes

1. **[TrafficGeneratorNode](Assets/Scripts/TrafficGeneratorNode.cs):**
   - Emits network requests with defined intervals.
   - Generates both standard traffic and malicious payloads to simulate attacks.

2. **[FirewallNode](Assets/Scripts/FirewallNode.cs):**
   - Evaluates incoming payloads against a configurable detection probability (`DetectionChance`).
   - Blocks and absorbs malicious traffic, preventing breaches while forwarding safe packets.

3. **[LoadBalancerNode](Assets/Scripts/LoadBalancerNode.cs):**
   - Balances heavy incoming request queues using round-robin routing across all connected output targets.

4. **[ComputeNode](Assets/Scripts/ComputeNode.cs):**
   - Acts as the backend server. Successfully processing legitimate requests earns revenue; processing uncaught malicious requests triggers a security breach penalty.

---

## 📊 Telemetry & Game Metrics

The simulation features a real-time observability engine via [NetworkTelemetry](Assets/Scripts/NetworkTelemetry.cs) and [HUDManager](Assets/Scripts/HUDManager.cs):

- **💰 Budget:** Operating capital. Increases with successful request completions; decreases with node upkeep costs and security breach penalties.
- **⚡ RPS (Requests Per Second):** Live throughput measurement of network traffic across the topology.
- **📈 Goodput (%):** Ratio of successfully completed legitimate requests relative to total generated requests.
- **⭐ Reputation:** SLA and customer trust rating. Slowly regenerates, but drops rapidly during dropped requests or system breaches.
- **🚫 Drops:** Requests dropped due to unhandled queues or full buffer capacities.
- **⚠️ Breaches:** Malicious attacks that bypassed perimeter security and hit compute servers.

---

## 📁 Project Structure

```
Assets/
├── Prefabs/             # Node prefabs (Client, Firewall, Load Balancer, Compute)
├── Scenes/              # Game scenes (SampleScene.unity)
├── Scripts/             # Core simulation logic & managers
│   ├── BaseNode.cs              # Base abstract class for network nodes
│   ├── ComputeNode.cs           # Backend processing node
│   ├── FirewallNode.cs          # Threat detection & packet filtering
│   ├── HUDManager.cs            # Live on-screen telemetry UI
│   ├── LoadBalancerNode.cs      # Round-robin request router
│   ├── NetworkRequest.cs        # Request payload definition
│   ├── NetworkTelemetry.cs      # System economics & metrics tracker
│   ├── NodePlacementManager.cs  # Player input & node placement
│   ├── TrafficGeneratorNode.cs  # Ingress traffic generator
│   └── TrafficVisualizer.cs     # Real-time packet path visualizer
└── Settings/            # URP and Project configuration assets
```
