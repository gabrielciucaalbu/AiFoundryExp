using Azure.AI.Agents.Persistent;
using Azure.Core;
using Azure.Identity;

namespace AiFoundryExp;

public class AgentFactory
{
    private readonly TokenCredential _credential;
    private readonly string _defaultEndpoint;
    private readonly string _defaultModelDeployment;
    private readonly Dictionary<string, PersistentAgentsClient> _clients = new();

    public AgentFactory(string defaultEndpoint, string defaultModelDeploymentName, TokenCredential? credential = null)
    {
        _credential = credential ?? new DefaultAzureCredential();
        _defaultEndpoint = defaultEndpoint;
        _defaultModelDeployment = defaultModelDeploymentName;
    }

    public PersistentAgentsClient GetClient(string endpoint)
    {
        if (!_clients.TryGetValue(endpoint, out PersistentAgentsClient? client))
        {
            client = new PersistentAgentsClient(endpoint, _credential);
            _clients[endpoint] = client;
        }
        return client;
    }

    public async Task<PersistentAgent> CreateAgentAsync(
        string name,
        string instructions,
        IEnumerable<ToolDefinition>? tools = null,
        ToolResources? resources = null,
        float? temperature = null,
        float? topP = null,
        string? endpoint = null,
        string? deploymentName = null)
    {
        tools ??= [];
        string ep = endpoint ?? _defaultEndpoint;
        string deployment = deploymentName ?? _defaultModelDeployment;
        PersistentAgentsClient client = GetClient(ep);

        await foreach (PersistentAgent existing in client.Administration.GetAgentsAsync())
        {
            if (string.Equals(existing.Name, name, StringComparison.OrdinalIgnoreCase))
            {
                return existing;
            }
        }

        return await client.Administration.CreateAgentAsync(
            model: deployment,
            name: name,
            instructions: instructions,
            tools: tools,
            toolResources: resources,
            temperature: temperature,
            topP: topP);
    }

    public async Task<PersistentAgent> UpdateAgentAsync(
        string agentId,
        string? name = null,
        string? instructions = null,
        IEnumerable<ToolDefinition>? tools = null,
        ToolResources? resources = null,
        float? temperature = null,
        float? topP = null,
        string? endpoint = null,
        string? deploymentName = null)
    {
        tools ??= [];
        string ep = endpoint ?? _defaultEndpoint;
        string deployment = deploymentName ?? _defaultModelDeployment;
        PersistentAgentsClient client = GetClient(ep);
        return await client.Administration.UpdateAgentAsync(
            assistantId: agentId,
            model: deployment,
            name: name,
            description: null,
            instructions: instructions,
            tools: tools,
            toolResources: resources,
            temperature: temperature,
            topP: topP,
            responseFormat: null,
            metadata: null);
    }

    public async Task<PersistentAgent> EnsureAgentAsync(AgentDefinition definition)
    {
        string ep = string.IsNullOrWhiteSpace(definition.Endpoint) ? _defaultEndpoint : definition.Endpoint!;
        string deployment = string.IsNullOrWhiteSpace(definition.DeploymentName) ? _defaultModelDeployment : definition.DeploymentName!;
        PersistentAgentsClient client = GetClient(ep);

        await foreach (PersistentAgent existing in client.Administration.GetAgentsAsync())
        {
            if (string.Equals(existing.Name, definition.Name, StringComparison.OrdinalIgnoreCase))
            {
                return await UpdateAgentAsync(
                    agentId: existing.Id,
                    name: definition.Name,
                    instructions: definition.Instructions,
                    temperature: definition.Temperature,
                    topP: definition.TopP,
                    endpoint: ep,
                    deploymentName: deployment);
            }
        }

        return await CreateAgentAsync(
            name: definition.Name,
            instructions: definition.Instructions,
            temperature: definition.Temperature,
            topP: definition.TopP,
            endpoint: ep,
            deploymentName: deployment);
    }
}
