require 'sinatra'
require 'haml'

configure do
  mime_type :unity3d, 'application/vnd.unity'
end

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