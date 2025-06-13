# AiFoundryExp# BusinessPlanAgent: AI-Powered Business Planning and Specification System

## Overview

BusinessPlanAgent is an intelligent multi-agent system designed to guide entrepreneurs and business teams through the complete journey from initial business concept to detailed technical specifications. The system leverages specialized AI agents working in concert to create comprehensive business plans, Software Requirements Specifications (SRS), and Functional Specification documents through an iterative, feedback-driven process.

## Key Features

The system provides end-to-end support for business and technical planning through automated document generation, intelligent questioning and feedback loops, seamless progression from business strategy to technical implementation details, and context-aware guidance adapted to user expertise levels. Each component maintains consistency across all generated documents while ensuring professional-quality outputs suitable for stakeholder presentations and development teams.

## System Architecture

### Core Components

The system employs a distributed agent architecture where each agent specializes in a specific domain while maintaining constant communication with other agents through a central orchestration layer. The architecture ensures scalability, maintainability, and the ability to add new specialized agents as needed.

### Agent Descriptions

**Orchestration Agent** serves as the system's central nervous system, managing workflow state, coordinating inter-agent communication, and ensuring proper sequencing of activities. This agent maintains the global context and determines which specialized agents should be activated based on the current project phase and user inputs.

**Business Strategy Agent** focuses on the foundational business planning aspects, including market analysis, competitive positioning, financial modeling, and strategic planning. This agent guides users through defining their value proposition, identifying target markets, developing revenue models, and creating growth strategies.

**Requirements Gathering Agent** bridges the gap between business vision and technical reality by translating business objectives into concrete technical requirements. This agent systematically explores user needs, system constraints, and success criteria while maintaining traceability between business goals and technical specifications.

**Technical Specification Agent** creates detailed Software Requirements Specifications that meet industry standards and best practices. This agent ensures technical feasibility, defines system architectures, specifies interfaces and integrations, and documents both functional and non-functional requirements.

**Functional Design Agent** transforms requirements into implementable functional specifications, including detailed workflow descriptions, user interface specifications, business logic documentation, and validation rules. This agent ensures that development teams have clear, actionable specifications to work from.

**User Interaction Agent** manages all communication with the user, formulating questions in accessible language, interpreting responses, and adapting communication style to match user expertise. This agent ensures users remain engaged throughout the process without becoming overwhelmed by technical complexity.

**Document Generation Agent** produces professional, consistently formatted documents from the collective work of all agents. This agent maintains version control, ensures terminology consistency, and incorporates feedback changes while generating outputs suitable for various stakeholders.

## Agent Cooperation Workflow

### Communication Protocol

The agents communicate through a sophisticated message-passing system coordinated by the Orchestration Agent. Each agent maintains its own knowledge base while sharing relevant information through a common data layer. This architecture ensures that agents can work independently while maintaining coherence across the entire system.

The repository includes a lightweight **MessageBus** implementation that allows agents to publish and subscribe to `AgentMessage` objects. Every agent registers with the bus on creation and can send structured messages to other agents without direct references, enabling loose coupling and simple extension of the protocol.

### Workflow Phases

The system operates through distinct phases, each involving specific agents working together to achieve phase objectives.

**Phase 1: Business Concept Development** begins when the Orchestration Agent activates the Business Strategy Agent and User Interaction Agent. The Business Strategy Agent formulates strategic questions about the business concept, market opportunity, and competitive landscape. The User Interaction Agent presents these questions in an approachable manner and interprets user responses. As users provide information, the Business Strategy Agent builds a comprehensive business model, identifying gaps and requesting clarification through the User Interaction Agent. The Orchestration Agent monitors progress and determines when sufficient business context exists to proceed.

**Phase 2: Requirements Discovery** involves the Requirements Gathering Agent taking the lead while maintaining close coordination with the Business Strategy Agent. The Requirements Gathering Agent analyzes the business model to identify implied technical needs and generates targeted questions about system capabilities, user workflows, and constraints. The User Interaction Agent continues to facilitate communication, ensuring technical concepts are presented appropriately. The Business Strategy Agent remains active to clarify business objectives when technical requirements seem misaligned with business goals.

**Phase 3: Technical Specification** sees the Technical Specification Agent become primary, working with inputs from both the Business Strategy and Requirements Gathering Agents. This agent transforms business and functional requirements into detailed technical specifications, consulting with the Requirements Gathering Agent to ensure completeness and accuracy. The User Interaction Agent presents technical decisions and trade-offs to users for validation. The Orchestration Agent ensures all technical decisions align with business constraints and objectives.

**Phase 4: Functional Design** activates the Functional Design Agent to create detailed specifications for system behavior and user interaction. This agent collaborates closely with the Technical Specification Agent to ensure functional designs are technically feasible. The User Interaction Agent presents mockups, workflows, and functional descriptions for user feedback. The Requirements Gathering Agent validates that all requirements are addressed in the functional specifications.

**Phase 5: Document Generation and Refinement** brings the Document Generation Agent to the forefront, compiling inputs from all other agents into cohesive, professional documents. This agent works iteratively, generating drafts that the User Interaction Agent presents for feedback. Based on user input, the Orchestration Agent may reactivate specific specialized agents to refine particular sections. The process continues until users approve all documents.

### Inter-Agent Data Flow

Agents share information through structured data objects that maintain context and traceability. Business objectives flow from the Business Strategy Agent through the Requirements Gathering Agent to the Technical Specification Agent, maintaining clear lineage. Technical constraints identified by the Technical Specification Agent flow back to influence business strategy refinements. User feedback collected by the User Interaction Agent is processed and routed to relevant specialized agents by the Orchestration Agent.

### Conflict Resolution

When agents produce conflicting recommendations, the Orchestration Agent facilitates resolution by identifying the source of conflict, engaging relevant agents to provide justification for their positions, and presenting trade-offs to the user through the User Interaction Agent. The system maintains a decision log to ensure consistency in future similar situations.

## Environment Variables

The application relies on a few environment variables to connect to your Azure resources and to determine the UID that the container should run as:

| Variable | Description |
|----------|-------------|
| `PROJECT_ENDPOINT` | Endpoint for your Azure AI Services project. |
| `MODEL_DEPLOYMENT_NAME` | Name of the model deployment to use. |
| `APP_UID` | UID that the container will run as. Typically set to your local user ID. |

## Running with Docker

Build the container image from the repository root:

```bash
docker build -t aifoundryexp -f AiFoundryExp/Dockerfile .
```

Run the container while providing the required environment variables. Mount the `input` and `output` folders if you want to persist data on the host:

```bash
docker run \
  -e PROJECT_ENDPOINT=<your-project-endpoint> \
  -e MODEL_DEPLOYMENT_NAME=<your-model-deployment> \
  -e APP_UID=$(id -u) \
  -v $(pwd)/input:/app/input \
  -v $(pwd)/output:/app/output \
  aifoundryexp
```
