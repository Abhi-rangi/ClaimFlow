#!/bin/bash

echo "🔧 Installing .NET SDK..."
echo ""
echo "This will install .NET SDK using Homebrew."
echo "You'll be prompted for your password."
echo ""

# Check if Homebrew is installed
if ! command -v brew &> /dev/null; then
    echo "❌ Homebrew is not installed."
    echo "Install it from: https://brew.sh"
    exit 1
fi

# Install .NET SDK
echo "📦 Installing .NET SDK (this may take a few minutes)..."
brew install --cask dotnet-sdk

# Check if installation was successful
if command -v dotnet &> /dev/null; then
    echo ""
    echo "✅ .NET SDK installed successfully!"
    echo "Version: $(dotnet --version)"
    echo ""
    echo "🚀 You can now run the backend with:"
    echo "   cd VerusClaims.API"
    echo "   dotnet restore"
    echo "   dotnet run"
else
    echo ""
    echo "⚠️  Installation may have completed, but dotnet command not found."
    echo "Try:"
    echo "   1. Close and reopen your terminal"
    echo "   2. Run: source ~/.zshrc (or ~/.bash_profile)"
    echo "   3. Verify with: dotnet --version"
fi

