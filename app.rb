require 'sinatra'
require 'haml'

get '/' do
  haml :index
end

__END__

@@ layout
%html
  = yield

@@ index
%ul
	%li
		%a{:href => "/flash/flash.html"}Flash
	%li
		%a{:href => "/web/web.html"}Web