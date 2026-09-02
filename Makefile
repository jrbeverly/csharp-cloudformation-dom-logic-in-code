# Makefile for the CloudFormation DOM scaffold.

.DEFAULT_GOAL := help
.PHONY: help build synth validate deploy delete

REGION := us-west-2
STACK_NAME := cfn-dom-logic-in-code-example

help: ## Show available targets
	@grep -E '^[a-zA-Z_-]+:.*?## .*$$' $(MAKEFILE_LIST) \
		| awk 'BEGIN {FS = ":.*?## "} {printf "  \033[36m%-12s\033[0m %s\n", $$1, $$2}'

build: ## Restore and build the console project
	dotnet build CfnDom.csproj

synth: build ## Run the synth entrypoint and write template.json
	dotnet run --project CfnDom.csproj --no-build

validate: ## Validate template.json with CloudFormation
	aws cloudformation validate-template --region $(REGION) --template-body file://template.json

deploy: ## Create the example stack and wait for completion
	aws cloudformation create-stack --region $(REGION) --stack-name $(STACK_NAME) --template-body file://template.json --capabilities CAPABILITY_NAMED_IAM
	aws cloudformation wait stack-create-complete --region $(REGION) --stack-name $(STACK_NAME)
	aws cloudformation describe-stacks --region $(REGION) --stack-name $(STACK_NAME) --query 'Stacks[0].{Status:StackStatus,Outputs:Outputs}'

delete: ## Delete the example stack and wait for completion
	aws cloudformation delete-stack --region $(REGION) --stack-name $(STACK_NAME)
	aws cloudformation wait stack-delete-complete --region $(REGION) --stack-name $(STACK_NAME)
