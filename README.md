# AiFoundryExp Minimal Agents

This proof-of-concept demonstrates a very small multi-agent workflow. Only three agents are included:

- **Orchestration Agent** – coordinates communication and activates the other agents.
- **Business Strategy Agent** – builds a simple business model from provided context.
- **Document Generation Agent** – writes a plain text business plan from the model.

The agents communicate through a lightweight message bus and can run locally or be backed by Azure AI agents. A sample configuration is provided under `AiFoundryExp/Configuration/agents.json`.

To run the console application:

```bash
# optional: set PROJECT_ENDPOINT and MODEL_DEPLOYMENT_NAME environment variables
cd AiFoundryExp
dotnet run --project AiFoundryExp
```

Input can be placed in `input/input.text`. Output files are written to the `output` directory.
